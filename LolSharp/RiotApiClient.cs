using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using LolSharp.Riot;
using LolSharp.Riot.Objects;
using RestSharp;

namespace LolSharp
{
    public class RiotApiClient
    {
        private readonly string _apiKey;
        private readonly RestClient _client;
        private string _region;
        private int _currentSummonerIndex;
        public List<Summoner> Summoners { get; set; }
        public Summoner CurrentSummoner { get; set; }
        
        public RiotApiClient(string key, string region)
        {
            _apiKey = key;
            _region = region.ToLower(CultureInfo.InvariantCulture);
            _client = new RestClient(String.Format(Endpoints.BaseUrl, region));
            Summoners = new List<Summoner>();
            CurrentSummoner = new Summoner();
            _currentSummonerIndex = 0;
            CheckApiKey(); // this just runs a standard method and if there is an error returned, it's most likely due to the API key
        }

        #region Private Methods

        private void CheckApiKey()
        {
            GetChampionData(1); // will throw AccessDeniedException if API key is not valid
        }

        private T Execute<T>(IRestRequest request, bool excludeSummonerId = false) where T : new()
        {
            if (!excludeSummonerId)
                request.AddUrlSegment("summonerId", CurrentSummoner.Id.ToString(CultureInfo.InvariantCulture));
            request.AddUrlSegment("region", _region);
            request.AddQueryParameter("api_key", _apiKey);

            var response = _client.Execute<T>(request);
            if (response.StatusCode.Equals((HttpStatusCode)RiotException.Good)) return response.Data;
            const string message = "Error retrieving response. Check inner description for more details.";
            if (response.StatusCode.Equals((HttpStatusCode)RiotException.AccessDenied)) throw new AccessDeniedException(message, response.ErrorException);
            if (response.StatusCode.Equals((HttpStatusCode)RiotException.RateLimitExceeded)) throw new RateLimitException(message, response.ErrorException);
            if (response.StatusCode.Equals((HttpStatusCode) RiotException.NotFound)) throw new NotFoundException(message, response.ErrorException);
            throw new ClientException(message, response.ErrorException);
        }

        #endregion

        #region Public Methods

        public string Region
        {
            get { return _region; }
            set
            {
                _region = value;
                Summoners = new List<Summoner>();
                CurrentSummoner = new Summoner();
                _currentSummonerIndex = 0;
                _client.BaseUrl = new Uri(String.Format(Endpoints.BaseUrl, value));
            }
        }

        public Summoner Search(string summonerName)
        {
            return Search(new List<string> { summonerName });
        }

        /// <summary>
        /// Searches for up to 40 summonerNames in _region; returns the first summoner and stores all in Summoners (use Next() to cycle through)
        /// </summary>
        /// <param name="summonerNames">List of strings that correspond to each summoner in _region</param>
        /// <returns>a Dictionary where each key is a summonerName string and each value is a Summoner</returns>
        public Summoner Search(IEnumerable<string> summonerNames)
        {
            var request = new RestRequest(Endpoints.PlayerByName);
            var summonerNamesList = summonerNames as IList<string> ?? summonerNames.ToList();
            request.AddUrlSegment("summonerNames",
                summonerNamesList.Aggregate("", (current, summonerName) => current + (summonerName + ","))); // converts IEnumerable<string> into "{0},{1},...{n-1}" string
            try
            {
                var summoners = Execute<Dictionary<string, Summoner>>(request, true); // json object is designed to work for 40 summoners, where each summonerName is a key for the relevant SummonerDto
                var summonersNotFound = new List<string>();
                foreach (var formattedSummonerName in summonerNamesList.Select(summonerName => summonerName.Replace(" ", "").ToLower(CultureInfo.InvariantCulture)))
                {
                    try
                    {
                        Summoners.Add(summoners[formattedSummonerName]);
                    }
                    catch (KeyNotFoundException) 
                    { 
                        //formattedSummonerName was not found on Riot's servers, can be thrown out; add these names to summonersNotFound list and throw that to the procedure call
                        summonersNotFound.Add(formattedSummonerName);
                    }
                }
                _currentSummonerIndex = 0;
                CurrentSummoner = Summoners.ElementAt(_currentSummonerIndex);
                if (summonersNotFound.Any()) throw new SummonersNotFoundException(summonersNotFound);
                return CurrentSummoner;
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return Search(summonerNamesList);
            }
        }

        public Summoner Next()
        {
            _currentSummonerIndex += 1;
            if (_currentSummonerIndex == Int32.MaxValue) _currentSummonerIndex = _currentSummonerIndex % Summoners.Count;
            CurrentSummoner = Summoners.ElementAt(_currentSummonerIndex % Summoners.Count);
            return CurrentSummoner;
        }

        public IEnumerable<Match> GetRankedMatchHistory(bool descending = true)
        {
            var request = new RestRequest(Endpoints.RankedMatchHistory);
            try
            {
                if (descending) return Execute<Dictionary<string, IEnumerable<Match>>>(request)["matches"].OrderByDescending(m => m.MatchCreation); // object looks like {"matches": [Match]}
                return Execute<Dictionary<string, IEnumerable<Match>>>(request)["matches"];
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetRankedMatchHistory(descending);
            }
        }

        public IEnumerable<Game> GetRecentMatchHistory(bool descending = true)
        {
            var request = new RestRequest(Endpoints.RecentMatchHistory);
            try
            {
                var gameHistory = Execute<GameHistory>(request);
                if (descending) gameHistory.Games = gameHistory.Games.OrderByDescending(g => g.CreateDate).ToList();
                return gameHistory.Games;
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetRecentMatchHistory(descending);
            }
        }

        public RankedStats GetRankedStats()
        {
            var request = new RestRequest(Endpoints.RankedStats);
            try
            {
                var stats = Execute<RankedStats>(request);
                foreach (var champion in stats.Champions) champion.Name = GetChampionData(champion.Id).Name;
                return stats;
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetRankedStats();
            }
        }

        public SummaryStats GetSummaryStats()
        {
            var request = new RestRequest(Endpoints.SummaryStats);
            try
            {
                return Execute<SummaryStats>(request);
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                return GetSummaryStats();
            }
        }

        public MatchDetails GetMatchDetails(long matchId)
        {
            var request = new RestRequest(Endpoints.MatchDetails);
            request.AddUrlSegment("matchId", matchId.ToString(CultureInfo.InvariantCulture));
            request.AddQueryParameter("includeTimeline", "true");
            try
            {
                return Execute<MatchDetails>(request, true);
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                return GetMatchDetails(matchId);
            }
        }

        public ChampionDtoList GetChampionData(bool all = false)
        {
            var request = new RestRequest(Endpoints.ChampionData);
            if (all) request.AddQueryParameter("champData", "all");
            try
            {
                return Execute<ChampionDtoList>(request, true);
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetChampionData(all);
            }
        }

        public ChampionDto GetChampionData(int championId, bool all = false)
        {
            var request = new RestRequest(Endpoints.ChampionDataById);
            if (all) request.AddQueryParameter("champData", "all");
            request.AddUrlSegment("id", championId.ToString(CultureInfo.InvariantCulture));
            try
            {
                return Execute<ChampionDto>(request, true);
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetChampionData(championId, all);
            }
        }

        public SummonerSpellDtoList GetSummonerSpellData(bool all = false)
        {
            var request = new RestRequest(Endpoints.SummonerSpellData);
            if (all) request.AddQueryParameter("spellData", "all");
            try
            {
                return Execute<SummonerSpellDtoList>(request, true);
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetSummonerSpellData(all);
            }
        }

        public SummonerSpellDto GetSummonerSpellData(int summonerSpellId, bool all = false)
        {
            var request = new RestRequest(Endpoints.SummonerSpellDataById);
            if (all) request.AddQueryParameter("champData", "all");
            request.AddUrlSegment("id", summonerSpellId.ToString(CultureInfo.InvariantCulture));
            try
            {
                return Execute<SummonerSpellDto>(request, true);
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetSummonerSpellData(summonerSpellId, all);
            }
        }

        public MasteryDtoList GetMasteryData(bool all = false)
        {
            var request = new RestRequest(Endpoints.MasteryData);
            if (all) request.AddQueryParameter("masteryListData", "all");
            try
            {
                return Execute<MasteryDtoList>(request, true);
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetMasteryData(all);
            }
        }

        public MasteryDto GetMasteryData(int masteryId, bool all = false)
        {
            var request = new RestRequest(Endpoints.MasteryDataById);
            if (all) request.AddQueryParameter("masteryData", "all");
            request.AddUrlSegment("id", masteryId.ToString(CultureInfo.InvariantCulture));
            try
            {
                return Execute<MasteryDto>(request, true);
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetMasteryData(masteryId, all);
            }
        }

        public RuneDtoList GetRuneData(bool all = false)
        {
            var request = new RestRequest(Endpoints.RuneData);
            if (all) request.AddQueryParameter("runeListData", "all");
            try
            {
                return Execute<RuneDtoList>(request, true);
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetRuneData(all);
            }
        }

        public RuneDto GetRuneData(int runeId, bool all = false)
        {
            var request = new RestRequest(Endpoints.RuneDataById);
            if (all) request.AddQueryParameter("runeData", "all");
            request.AddUrlSegment("id", runeId.ToString(CultureInfo.InvariantCulture));
            try
            {
                return Execute<RuneDto>(request, true);
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetRuneData(runeId, all);
            }
        }

        public ItemDtoList GetItemData(bool all = false)
        {
            var request = new RestRequest(Endpoints.ItemData);
            if (all) request.AddQueryParameter("itemListData", "all");
            try
            {
                return Execute<ItemDtoList>(request, true);
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetItemData(all);
            }
        }

        public ItemDto GetItemData(int itemId, bool all = false)
        {
            var request = new RestRequest(Endpoints.ItemDataById);
            if (all) request.AddQueryParameter("itemData", "all");
            request.AddUrlSegment("id", itemId.ToString(CultureInfo.InvariantCulture));
            try
            {
                return Execute<ItemDto>(request, true);
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetItemData(itemId, all);
            }
        }

        public IEnumerable<LeagueDto> GetLeagueData(bool allSummoners = false)
        {
            var request = new RestRequest(Endpoints.LeagueData);
            if (allSummoners)
            {
                var currentSummoners = Summoners;
                if (currentSummoners.Count > 10)
                {
                    var restSummoners = currentSummoners.Skip(10).ToList();
                    currentSummoners = currentSummoners.Take(10).ToList();
                    var currentTempClient = new RiotApiClient(_apiKey, Region) {Summoners = currentSummoners};
                    var restTempClient = new RiotApiClient(_apiKey, Region) {Summoners = restSummoners};
                    var leagueData = currentTempClient.GetLeagueData(true).ToList();
                    leagueData.AddRange(restTempClient.GetLeagueData(true));
                    return leagueData;
                }
                request.AddUrlSegment("summonerIds",
                        Summoners.Aggregate("",
                            (current, summoner) => current + (summoner.Id.ToString(CultureInfo.InvariantCulture) + ",")));
            }
            else
            {
                request.AddUrlSegment("summonerIds", CurrentSummoner.Id.ToString(CultureInfo.InvariantCulture));
            }

            try
            {
                if (!allSummoners)
                    return
                        Execute<Dictionary<string, IEnumerable<LeagueDto>>>(request, true)[
                            CurrentSummoner.Id.ToString(CultureInfo.InvariantCulture)];
                var results = Execute<Dictionary<string, IEnumerable<LeagueDto>>>(request, true);
                var returnResults = new List<LeagueDto>();
                foreach (var summoner in Summoners)
                {
                    try
                    {
                        returnResults.AddRange(results[summoner.Id.ToString(CultureInfo.InvariantCulture)]);
                    }
                    catch (KeyNotFoundException)
                    {
                        returnResults.AddRange(new List<LeagueDto>());
                    }
                }
                return returnResults;
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetLeagueData(allSummoners);
            }
        }

        public CurrentGameInfo GetCurrentGameInfo()
        {
            var request = new RestRequest(Endpoints.CurrentGameInfo);
            string platformId;
            switch (_region)
            {
                case RegionId.Na:
                    platformId = PlatformId.Na;
                    break;
                case RegionId.Br:
                    platformId = PlatformId.Br;
                    break;
                case RegionId.Eune:
                    platformId = PlatformId.Eune;
                    break;
                case RegionId.Euw:
                    platformId = PlatformId.Euw;
                    break;
                case RegionId.Kr:
                    platformId = PlatformId.Kr;
                    break;
                case RegionId.Lan:
                    platformId = PlatformId.Lan;
                    break;
                case RegionId.Las:
                    platformId = PlatformId.Las;
                    break;
                case RegionId.Oce:
                    platformId = PlatformId.Oce;
                    break;
                case RegionId.Ru:
                    platformId = PlatformId.Ru;
                    break;
                case RegionId.Tr:
                    platformId = PlatformId.Tr;
                    break;
                default:
                    throw new ClientException(String.Format("Platform id could not be found for region '{0}'", _region));
            }
            request.AddUrlSegment("platformId", platformId);
            try
            {
                return Execute<CurrentGameInfo>(request);
            }
            catch (RateLimitException)
            {
                Thread.Sleep(1000);
                return GetCurrentGameInfo();
            }
        }

        #endregion
    }

    [Serializable]
    public class ClientException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ClientException()
        {
        }

        public ClientException(string message) : base(message)
        {
        }

        public ClientException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ClientException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class RateLimitException : ClientException
    {
        public RateLimitException()
        {
        }

        public RateLimitException(string message) : base(message)
        {
        }

        public RateLimitException(string message, Exception inner) : base(message, inner)
        {
        }

        protected RateLimitException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class AccessDeniedException : ClientException
    {
        public AccessDeniedException()
        {
        }

        public AccessDeniedException(string message)
            : base(message)
        {
        }

        public AccessDeniedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected AccessDeniedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class NotFoundException : ClientException
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// This is different than just a 404 Not Found Exception in that Riot's API allows a search for 40 summoners at a time, and won't
    /// error out if just one summoner of the 40 is actually found (the API will just return the one summoner in this case). A 404 Exception
    /// will not be thrown so SummonersNotFoundException accounts for this scenario
    /// </summary>
    public class SummonersNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //
        public IEnumerable<string> SummonersNotFoundList { get; set; } 

        public SummonersNotFoundException()
        {
        }

        public SummonersNotFoundException(IEnumerable<string> summonersNotFoundList)
        {
            SummonersNotFoundList = summonersNotFoundList;
        }

        public SummonersNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected SummonersNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
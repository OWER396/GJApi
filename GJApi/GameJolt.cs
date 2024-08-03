using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GJApi
{
    public class GameJolt
    {
        private static readonly string gj = "[GameJolt] ";
        private static readonly string apiLink = "https://api.gamejolt.com/api/game/v1_2/";
        private static int GameID;
        private static string PrivateKey = String.Empty;
        private static readonly HttpClient client = new HttpClient();
        public static List<User> FetchedUsers = new List<User>();
        public static List<Trophy> FetchedTrophies = new List<Trophy>();
        public static List<Score> FetchedScores = new List<Score>();
        public static List<Table> FetchedTables = new List<Table>();
        public static List<Key> FetchedKeys = new List<Key>();
        public static List<Friend> FetchedFriends = new List<Friend>();
        public static ServerTime FetchedServerTime = new ServerTime();
        private static string Username = String.Empty;
        private static string UserToken = String.Empty;

        private static async Task<string> Get(string url)
        {
            var response = client.GetStringAsync(url);

            return await response;
        }

        private static string GetMD5(string input)
        {
            using(MD5 md5 = MD5.Create())
            {
                byte[] result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        public enum Operation {
            Add,
            Subtract,
            Multiply,
            Divide,
            Append,
            Prepend
        }

        public static void SetGame(int game_id, string private_key)
        {
            GameID = game_id;
            PrivateKey = private_key;
            Console.WriteLine($"{gj}Game set successfully");
        }

        public static void Login(string username, string user_token)
        {
            string link = apiLink + $"users/auth?game_id={GameID}&username={username}&user_token={user_token}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Username = username;
                UserToken = user_token;
                Console.WriteLine($"{gj}User successfully logged in");
            }else
            {
                Console.WriteLine($"{gj}Login failed! Reason: {response["message"]}");
            }
        }

        public static void FetchUserByUsername(string username)
        {
            string link = apiLink + $"users?game_id={GameID}&username={username}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                FetchedUsers = JsonSerializer.Deserialize<List<User>>(response["users"]);
                Console.WriteLine($"{gj}Successfully fetched user");
            }
            else
            {
                Console.WriteLine($"{gj}User fetch failed! Reason: {response["message"]}");
            }
        }

        public static void FetchUserByID(int id)
        {
            string link = apiLink + $"users?game_id={GameID}&user_id={id}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                FetchedUsers = JsonSerializer.Deserialize<List<User>>(response["users"]);
                Console.WriteLine($"{gj}Successfully fetched user");
            }
            else
            {
                Console.WriteLine($"{gj}User fetch failed! Reason: {response["message"]}");
            }
        }

        public static void FetchUsersByID(string id)
        {
            string link = apiLink + $"users?game_id={GameID}&user_id={id}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                FetchedUsers = JsonSerializer.Deserialize<List<User>>(response["users"]);
                Console.WriteLine($"{gj}Successfully fetched users");
            }
            else
            {
                Console.WriteLine($"{gj}User fetch failed! Reason: {response["message"]}");
            }
        }

        public static void OpenSession()
        {
            string link = apiLink + $"sessions/open?game_id={GameID}&username={Username}&user_token={UserToken}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Session opened");
            }else
            {
                Console.WriteLine($"{gj}Session open failed! Reason: {response["message"]}");
            }
        }

        public static void PingSession()
        {
            string link = apiLink + $"sessions/ping?game_id={GameID}&username={Username}&user_token={UserToken}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Session successfully pinged");
            }
            else
            {
                Console.WriteLine($"{gj}Session ping failed! Reason: {response["message"]}");
            }
        }

        public static void CloseSession()
        {
            string link = apiLink + $"sessions/close?game_id={GameID}&username={Username}&user_token={UserToken}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Session closed");
            }else
            {
                Console.WriteLine($"{gj}Session close failed! Reason: {response["message"]}");
            }
        }

        public static bool CheckSession()
        {
            bool isOpen;
            string link = apiLink + $"sessions/check?game_id={GameID}&username={Username}&user_token={UserToken}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                isOpen = true;
            }else
            {
                isOpen = false;
            }
            return isOpen;
        }

        public static void FetchTrophies(int? trophy_id = null, bool? achieved = null)
        {
            string link = apiLink + $"trophies?game_id={GameID}&username={Username}&user_token={UserToken}";
            if (achieved != null) link += $"&achieved={achieved}";
            if (trophy_id != null) link += $"&trophy_id={trophy_id}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            Console.WriteLine(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                FetchedTrophies = JsonSerializer.Deserialize<List<Trophy>>(response["trophies"]);
                Console.WriteLine($"{gj}Fetched trophies successfully");
            }else
            {
                Console.WriteLine($"{gj}Fetch trophies failed! Reason: {response["message"]}");
            }
        }

        public static void AchieveTrophy(int trophy_id)
        {
            string link = apiLink + $"trophies/add-achieved?game_id={GameID}&username={Username}&user_token={UserToken}&trophy_id={trophy_id}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Trophy achieved successfully");
            }else
            {
                Console.WriteLine($"{gj}Trophy achieve failed! Reason: {response["message"]}");
            }
        }

        public static void RemoveTrophy(int trophy_id)
        {
            string link = apiLink + $"trophies/remove-achieved?game_id={GameID}&username={Username}&user_token={UserToken}&trophy_id={trophy_id}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Trophy removed successfully");
            }else
            {
                Console.WriteLine($"{gj}Trophy remove failed! Reason: {response["message"]}");
            }
        }

        public static void FetchScores(int table_id, int? better_than = null, int? worse_than = null, int? limit = 10)
        {
            string link = apiLink + $"scores?game_id={GameID}";
            if (limit != null) link += $"&limit={limit}";
            link += $"&table_id={table_id}";
            if (better_than != null) link += $"&better_than={better_than}";
            if (worse_than != null) link += $"&worse_than={worse_than}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            Console.WriteLine(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                FetchedScores = JsonSerializer.Deserialize<List<Score>>(response["scores"]);
                Console.WriteLine($"{gj}Scores fetched successfully");
            }else
            {
                Console.WriteLine($"{gj}Score fetch failed! Reason: {response["message"]}");
            }
        }

        public static void FetchUserScores(int table_id, int? better_than = null, int? worse_than = null, int? limit = 10)
        {
            string link = apiLink + $"scores?game_id={GameID}";
            if(limit != null) link += $"&limit={limit}";
            link += $"&table_id={table_id}&username={Username}&user_token={UserToken}";
            if(better_than != null) link += $"&better_than={better_than}";
            if (worse_than != null) link += $"&worse_than={worse_than}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                FetchedScores = JsonSerializer.Deserialize<List<Score>>(response["scores"]);
                Console.WriteLine($"{gj}Scores fetched successfully");
            }else
            {
                Console.WriteLine($"{gj}Score fetch failed! Reason: {response["message"]}");
            }
        }

        public static void FetchGuestScores(int table_id, string guest, int? better_than = null, int? worse_than = null, int? limit = 10)
        {
            string link = apiLink + $"scores?game_id={GameID}";
            if (limit != null) link += $"&limit={limit}";
            link += $"&table_id={table_id}&guest={guest}";
            if (better_than != null) link += $"&better_than={better_than}";
            if (worse_than != null) link += $"&worse_than={worse_than}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                FetchedScores = JsonSerializer.Deserialize<List<Score>>(response["scores"]);
                Console.WriteLine($"{gj}Scores fetched successfully");
            }else
            {
                Console.WriteLine($"{gj}Score fetch failed! Reason: {response["message"]}");
            }
        }

        public static void FetchTables()
        {
            string link = apiLink + $"scores/tables?game_id={GameID}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                FetchedTables = JsonSerializer.Deserialize<List<Table>>(response["tables"]);
                Console.WriteLine($"{gj}Tables fetched successfully");
            }else
            {
                Console.WriteLine($"{gj}Table fetch failed! Reason: {response["message"]}");
            }
        }

        public static void AddUserScore(int table_id, string score, int sort, string? extra_data = null)
        {
            string link = apiLink + $"scores/add?game_id={GameID}&username={Username}&user_token={UserToken}&score={score}&sort={sort}";
            if (extra_data != null) link += $"&extra_data={extra_data}";
            link += $"&table_id={table_id}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Score added successfully");
            }else
            {
                Console.WriteLine($"{gj}Score add failed! Reason: {response["message"]}");
            }
        }

        public static void AddGuestScore(int table_id, string guest, string score, int sort, string? extra_data = null)
        {
            string link = apiLink + $"scores/add?game_id={GameID}&guest={guest}&score={score}&sort={sort}";
            if (extra_data != null) link += $"&extra_data={extra_data}";
            link += $"&table_id={table_id}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Score added successfully");
            }
            else
            {
                Console.WriteLine($"{gj}Score add failed! Reason: {response["message"]}");
            }
        }

        public static int? GetScoreRank(int table_id, int sort)
        {
            int? result = null;
            string link = apiLink + $"scores/get-rank?game_id={GameID}&sort={sort}&table_id={table_id}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                result = (int)response["rank"];
            }else
            {
                Console.WriteLine($"{gj}Get score rank failed! Reason: {response["message"]}");
            }
            return result;
        }

        public static void SetUserDataInt(string key, int data)
        {
            string link = apiLink + $"data-store/set?game_id={GameID}&key={key}&data={data}&username={Username}&user_token={UserToken}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Data successfully set");
            }else
            {
                Console.WriteLine($"{gj}Data set failed! Reason: {response["message"]}");
            }
        }

        public static void SetUserDataString(string key, string data)
        {
            string link = apiLink + $"data-store/set?game_id={GameID}&key={key}&data={data}&username={Username}&user_token={UserToken}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Data successfully set");
            }
            else
            {
                Console.WriteLine($"{gj}Data set failed! Reason: {response["message"]}");
            }
        }

        public static void SetGlobalDataInt(string key, int data)
        {
            string link = apiLink + $"data-store/set?game_id={GameID}&key={key}&data={data}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Data successfully set");
            }
            else
            {
                Console.WriteLine($"{gj}Data set failed! Reason: {response["message"]}");
            }
        }

        public static void SetGlobalDataString(string key, string data)
        {
            string link = apiLink + $"data-store/set?game_id={GameID}&key={key}&data={data}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Data successfully set");
            }
            else
            {
                Console.WriteLine($"{gj}Data set failed! Reason: {response["message"]}");
            }
        }

        public static int FetchUserDataInt(string key)
        {
            int result = -1;
            string link = apiLink + $"data-store?game_id={GameID}&key={key}&username={Username}&user_token={UserToken}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                result = (int)response["data"];
                Console.WriteLine($"{gj}Data fetched successfully");
            }else
            {
                Console.WriteLine($"{gj}Data fetch failed! Reason: {response["message"]}");
            }
            return result;
        }

        public static string FetchUserDataString(string key)
        {
            string result = String.Empty;
            string link = apiLink + $"data-store?game_id={GameID}&key={key}&username={Username}&user_token={UserToken}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                result = response["data"].ToString();
                Console.WriteLine($"{gj}Data fetched successfully");
            }
            else
            {
                Console.WriteLine($"{gj}Data fetch failed! Reason: {response["message"]}");
            }
            return result;
        }

        public static string FetchGlobalData(string key)
        {
            string result = String.Empty;
            string link = apiLink + $"data-store?game_id={GameID}&key={key}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                result = response["data"].ToString();
                Console.WriteLine($"{gj}Data fetched successfully");
            }
            else
            {
                Console.WriteLine($"{gj}Data fetch failed! Reason: {response["message"]}");
            }
            return result;
        }

        public static void UpdateUserDataString(string key, Operation operation, string value)
        {
            string op = String.Empty;
            switch (operation)
            {
                case Operation.Add:
                    op = "add";
                    break;
                case Operation.Subtract:
                    op = "subtract";
                    break;
                case Operation.Multiply:
                    op = "multiply";
                    break;
                case Operation.Divide:
                    op = "divide";
                    break;
                case Operation.Append:
                    op = "append";
                    break;
                case Operation.Prepend:
                    op = "prepend";
                    break;
            }
            string link = apiLink + $"data-store/update?game_id={GameID}&key={key}&username={Username}&user_token={UserToken}&operation={op}&value={value}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Data updated successfully");
            }else
            {
                Console.WriteLine($"{gj}Data update failed! Reason: {response["message"]}");
            }
        }

        public static void UpdateUserDataInt(string key, Operation operation, int value)
        {
            string op = String.Empty;
            switch (operation)
            {
                case Operation.Add:
                    op = "add";
                    break;
                case Operation.Subtract:
                    op = "subtract";
                    break;
                case Operation.Multiply:
                    op = "multiply";
                    break;
                case Operation.Divide:
                    op = "divide";
                    break;
                case Operation.Append:
                    op = "append";
                    break;
                case Operation.Prepend:
                    op = "prepend";
                    break;
            }
            string link = apiLink + $"data-store/update?game_id={GameID}&key={key}&username={Username}&user_token={UserToken}&operation={op}&value={value}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Data updated successfully");
            }
            else
            {
                Console.WriteLine($"{gj}Data update failed! Reason: {response["message"]}");
            }
        }

        public static void UpdateGlobalDataString(string key, Operation operation, string value)
        {
            string op = String.Empty;
            switch (operation)
            {
                case Operation.Add:
                    op = "add";
                    break;
                case Operation.Subtract:
                    op = "subtract";
                    break;
                case Operation.Multiply:
                    op = "multiply";
                    break;
                case Operation.Divide:
                    op = "divide";
                    break;
                case Operation.Append:
                    op = "append";
                    break;
                case Operation.Prepend:
                    op = "prepend";
                    break;
            }
            string link = apiLink + $"data-store/update?game_id={GameID}&key={key}&operation={op}&value={value}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Data updated successfully");
            }
            else
            {
                Console.WriteLine($"{gj}Data update failed! Reason: {response["message"]}");
            }
        }

        public static void UpdateGlobalDataInt(string key, Operation operation, int value)
        {
            string op = String.Empty;
            switch(operation)
            {
                case Operation.Add:
                    op = "add";
                    break;
                case Operation.Subtract:
                    op = "subtract";
                    break;
                case Operation.Multiply:
                    op = "multiply";
                    break;
                case Operation.Divide:
                    op = "divide";
                    break;
                case Operation.Append:
                    op = "append";
                    break;
                case Operation.Prepend:
                    op = "prepend";
                    break;
            }
            string link = apiLink + $"data-store/update?game_id={GameID}&key={key}&operation={op}&value={value}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Data updated successfully");
            }
            else
            {
                Console.WriteLine($"{gj}Data update failed! Reason: {response["message"]}");
            }
        }

        public static void RemoveUserData(string key)
        {
            string link = apiLink + $"data-store/remove?game_id={GameID}&key={key}&username={Username}&user_token={UserToken}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Data removed successfully");
            }
            else
            {
                Console.WriteLine($"{gj}Data remove failed! Reason: {response["message"]}");
            }
        }

        public static void RemoveGlobalData(string key)
        {
            string link = apiLink + $"data-store/remove?game_id={GameID}&key={key}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                Console.WriteLine($"{gj}Data removed successfully");
            }
            else
            {
                Console.WriteLine($"{gj}Data remove failed! Reason: {response["message"]}");
            }
        }

        public static void FetchGlobalKeys(string? pattern = null)
        {
            string link = apiLink + $"data-store/get-keys?game_id={GameID}";
            if (pattern != null) link += $"&pattern={pattern}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                FetchedKeys = JsonSerializer.Deserialize<List<Key>>(response["keys"]);
                Console.WriteLine($"{gj}Keys fetched successfully");
            }
            else
            {
                Console.WriteLine($"{gj}Key fetch failed! Reason: {response["message"]}");
            }
        }

        public static void FetchUserKeys(string? pattern = null)
        {
            string link = apiLink + $"data-store/get-keys?game_id={GameID}";
            if (pattern != null) link += $"&pattern={pattern}";
            link += $"&username={Username}&user_token={UserToken}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                FetchedKeys = JsonSerializer.Deserialize<List<Key>>(response["keys"]);
                Console.WriteLine($"{gj}Keys fetched successfully");
            }
            else
            {
                Console.WriteLine($"{gj}Key fetch failed! Reason: {response["message"]}");
            }
        }

        public static void FetchFriends()
        {
            string link = apiLink + $"friends?game_id={GameID}&username={Username}&user_token={UserToken}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject()["response"];
            if (response["success"].ToString() == "true")
            {
                FetchedFriends = JsonSerializer.Deserialize<List<Friend>>(response["friends"]);
                Console.WriteLine($"{gj}Friends fetched successfully");
            }else
            {
                Console.WriteLine($"{gj}Friends fetch failed! Reason: {response["message"]}");
            }
        }

        public static void FetchServerTime()
        {
            string link = apiLink + $"time?game_id={GameID}";
            string md5 = GetMD5(link + PrivateKey);
            var awaiter = Get(link + $"&signature={md5}");
            var response = JsonObject.Parse(awaiter.Result).AsObject();
            var response2 = (JsonObject)response["response"];
            if (response2["success"].ToString() == "true")
            {
                response2.Remove("success");
                FetchedServerTime = JsonSerializer.Deserialize<ServerTime>(response2);
                Console.WriteLine($"{gj}Server time fetched successfully");
            }else
            {
                Console.WriteLine($"{gj}Server time fetch failed! Reason: {response2["message"]}");
            }
        }
    }

    public class User
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int id { get; set; }
        public string? type { get; set; }
        public string? username { get; set; }
        public string? avatar_url { get; set; }
        public string? signed_up { get; set; }
        public int signed_up_timestamp { get; set; }
        public string? last_logged_in { get; set; }
        public int last_logged_in_timestamp { get; set; }
        public string? status { get; set; }
        public string? developer_name { get; set; }
        public string? developer_website { get; set; }
        public string? developer_description { get; set; }
    }

    public class Trophy
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int id { get; set;}
        public string? title { get; set; }
        public string? difficulty { get; set; }
        public string? description { get; set; }
        public string? image_url { get; set; }
        public string? achieved { get; set; }
    }

    public class Score
    {
        public string? score { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int sort { get; set; }
        public string? extra_data { get; set; }
        public string? user { get; set; }
        public string? user_id { get; set; }
        public string? guest { get; set; }
        public string? stored { get; set; }
        public int stored_timestamp { get; set; }
    }

    public class Table
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public string? primary { get; set; }
    }

    public class Key
    {
        public string? key { get; set; }
    }

    public class Friend
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int friend_id { get; set; }
    }

    public class ServerTime
    {
        public int timestamp { get; set; }
        public string? timezone { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int year { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int month { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int day { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int hour { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int minute { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int second { get; set; }
    }
}

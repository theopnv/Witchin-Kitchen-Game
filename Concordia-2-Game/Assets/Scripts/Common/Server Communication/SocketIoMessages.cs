using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace con2.messages
{

    /// <summary>
    /// Implementation of the protocol used to communicate with the server
    /// https://docs.google.com/document/d/1Emv4Ce7AKEK7Xwwe3QJfFm7377PNbc0GcI2_kTas4uA/edit#
    /// </summary>

    public class Command
    {
        // Requests
        public const string MAKE_GAME = "makeGame";
        public const string REGISTER_PLAYERS = "registerPlayers";
        public const string QUIT_GAME = "quitGame";

        // Responses
        public const string MESSAGE = "message";
        public const string GAME_CREATED = "gameCreated";
        public const string GAME_UPDATE = "gameUpdate";
    }

    public enum Code
    {
        make_game_error = 211,

        register_players_success = 220,
        register_players_error = 221,

        quit_game_success = 230,
        quit_game_error = 231,
    }

    public class Base
    {
        public Code code;
        public string content;
    }

    public class Game
    {
        public string id;
        public string mainSocketID;
        public List<Player> players;
        public List<string> viewers; // socket IDs
    }

    public class Player
    {
        public string color;
        public string name;
    }

    public class Players
    {
        public List<Player> players;
    }
}

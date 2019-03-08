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
        // Socket IO reserved
        public const string CONNECT = "connect";
        public const string DISCONNECT = "disconnect";

        // Emitted
        public const string MAKE_GAME = "makeGame";
        public const string REGISTER_PLAYERS = "registerPlayers";
        public const string QUIT_GAME = "quitGame";
        public const string LAUNCH_POLL = "launchPoll";
        public const string LAUNCH_SPELL_CAST = "launchSpellCast";

        // Received
        public const string MESSAGE = "message";
        public const string GAME_CREATED = "gameCreated";
        public const string GAME_UPDATE = "gameUpdate";
        public const string RECEIVE_VOTES = "event";
        public const string SPELL_CAST_REQUEST = "spell";
    }

    public enum Code
    {
        make_game_error = 211,

        register_players_success = 220,
        register_players_error = 221,

        quit_game_success = 230,
        quit_game_error = 231,

        launch_poll_success = 260,
        launch_poll_error = 261,

        launch_spell_cast_success = 280,
        launch_spell_cast_error = 281,
    }

    public class Base
    {
        public Code code;
        public string content;
    }

    public class Game
    {
        public string id;
        public string pin;
        public string mainSocketID;
        public List<Player> players;
        public List<Viewer> viewers;
    }

    public class Viewer
    {
        public string socketId;
        public string color;
        public string name;
    }

    public class Player
    {
        public int id;
        public string color;
        public string name;
        public int score;
    }

    public class Players
    {
        public List<Player> players;
    }

    public class PollChoices
    {
        public List<Event> events;
        public string deadline;
        public int duration;
    }

    public class Event
    {
        public int id;
        public int votes;
    }

    public class GameOutcome
    {
        public bool gameFinished;
        public Player winner;
    }

    public class Spell
    {
        public int spellId;
        public Player targetedPlayer;
        public Viewer caster;
    }

    public class GameForViewer
    {
        public Game game;
        public Viewer viewer;
    }

    public static class SocketInfo
    {
        public const string SUFFIX_ADDRESS = "socket.io/?EIO=4&transport=websocket";
    }
}


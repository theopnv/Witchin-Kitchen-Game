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
        public const string END_GAME = "endGame";
        public const string LAUNCH_POLL = "launchPoll";
        public const string LAUNCH_SPELL_CAST = "launchSpellCast";
        public const string SEND_GAME_STATE = "updateGameState";
        public const string GAME_OUTCOME = "gameOutcome";

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

        rematch_success = 320,
        rematch_error = 321,
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
        public int potions;
        public int ingredients;
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
        public Player[] leaderboards;
    }

    public class SpellRequest
    {
        public Viewer targetedViewer;
        public Player fromPlayer;
    }

    public class Spell
    {
        public int spellId;
        public Player targetedPlayer;
        public Viewer caster;
    }

    public class EndGame
    {
        public bool doRematch;
    }

    public static class SocketInfo
    {
        public const string SUFFIX_ADDRESS = "socket.io/?EIO=4&transport=websocket";
    }
}


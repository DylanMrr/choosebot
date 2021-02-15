using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChooseBot
{
    public class StateService
    {
        public Dictionary<long, State> States { get; set; } = new Dictionary<long, State>();
    }

    public enum State
    {
        Rules,
        StartGame,
        Game,
        Additional,
        None
    }
}

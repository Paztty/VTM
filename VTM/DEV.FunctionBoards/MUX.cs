using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.Devices
{
    public class Channel
    {
        public int Channel_P_0_48 { get; set; }
        public int Channel_N_0_48 { get; set; }

        public int Channel_P_49_96 { get; set; }
        public int Channel_N_49_96 { get; set; }

        public Channel(int p_a, int n_a, int p_b, int n_b)
        {
            Channel_P_0_48 = p_a;
            Channel_N_0_48 = n_a;
            Channel_P_49_96 = p_b;
            Channel_N_49_96 = n_b;
        }
    }
    public class MUX
    {
        public List<Channel> Channels { get; set; } = new List<Channel>()
        {
        new Channel(1 ,48 ,49 ,96 ),
        new Channel(2 ,48 ,49 ,96 ),
        new Channel(3 ,48 ,49 ,96 ),
        new Channel(4 ,48 ,49 ,96 ),
        new Channel(5 ,48 ,49 ,96 ),
        new Channel(6 ,48 ,49 ,96 ),
        new Channel(7 ,48 ,49 ,96 ),
        new Channel(8 ,48 ,49 ,96 ),
        new Channel(9 ,48 ,49 ,96 ),
        new Channel(10,48 ,49 ,96 ),
        new Channel(11,48 ,49 ,96 ),
        new Channel(12,48 ,49 ,96 ),
        new Channel(13,48 ,49 ,96 ),
        new Channel(14,48 ,49 ,96 ),
        new Channel(15,48 ,49 ,96 ),
        new Channel(16,48 ,49 ,96 ),
        new Channel(17,48 ,49 ,96 ),
        new Channel(18,48 ,49 ,96 ),
        new Channel(19,48 ,49 ,96 ),
        new Channel(20,48 ,49 ,96 ),
        new Channel(21,48 ,49 ,96 ),
        new Channel(22,48 ,49 ,96 ),
        new Channel(23,48 ,49 ,96 ),
        new Channel(24,48 ,49 ,96 ),
        new Channel(25,48 ,49 ,96 ),
        new Channel(26,48 ,49 ,96 ),
        new Channel(27,48 ,49 ,96 ),
        new Channel(28,48 ,49 ,96 ),
        new Channel(29,48 ,49 ,96 ),
        new Channel(30,48 ,49 ,96 ),
        new Channel(31,48 ,49 ,96 ),
        new Channel(32,48 ,49 ,96 ),
        new Channel(33,48 ,49 ,96 ),
        new Channel(34,48 ,49 ,96 ),
        new Channel(35,48 ,49 ,96 ),
        new Channel(36,48 ,49 ,96 ),
        new Channel(37,48 ,49 ,96 ),
        new Channel(38,48 ,49 ,96 ),
        new Channel(39,48 ,49 ,96 ),
        new Channel(40,48 ,49 ,96 ),
        new Channel(41,48 ,49 ,96 ),
        new Channel(42,48 ,49 ,96 ),
        new Channel(43,48 ,49 ,96 ),
        new Channel(44,48 ,49 ,96 ),
        new Channel(45,48 ,49 ,96 ),
        new Channel(46,48 ,49 ,96 ),
        new Channel(47,48 ,49 ,96 ),
        };
    }
}

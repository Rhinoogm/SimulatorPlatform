using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace SimulatorPlatform
{
    public class Mediator
    {
        private ConcurrentDictionary<String, List<Colleague>> dColleague;
        private List<Colleague> lColleague;
        public Mediator()
        {
            dColleague = new ConcurrentDictionary<string, List<Colleague>>();
            lColleague = new List<Colleague>();
        }
        public void registerColleague(String _sColleagueListName, Colleague _cColleague)
        {
            if (dColleague.ContainsKey(_sColleagueListName) == false)
            {
                dColleague.TryAdd(_sColleagueListName, new List<Colleague>());
            }
            dColleague[_sColleagueListName].Add(_cColleague);
        }
        public void deleteColleague(String _sColleagueListName, Colleague _cColleague)
        {
            if (dColleague.ContainsKey(_sColleagueListName) == true)
            {
                if (dColleague[_sColleagueListName].Contains(_cColleague) == true)
                {
                    dColleague[_sColleagueListName].Remove(_cColleague);
                }
                else
                {
                    Console.WriteLine("[ERROR] ConcreteMediator :: deleteColleague :: {0}!!", _cColleague);
                }
            }
            else
            {
                Console.WriteLine("[ERROR] ConcreteMediator :: deleteColleague :: {0}!!", _sColleagueListName);
            }
        }
        public void publishData(String _order, Object _data)
        {
            // order => "SOURCE:DESTINATION:ORDER"
            String[] orderArray = _order.Split(':');
            String sourceName = orderArray[0];
            String destinationName = orderArray[1];
            // 각 Colleague들이 가지고 있는 Colleague List를 검색한다.
            if (dColleague.ContainsKey(sourceName) == true)
            {
                // Colleague List 내의 Colleague 들에게 Data를 Publish한다.
                foreach (var _cColleague in dColleague[sourceName])
                {
                    _cColleague.getData(_order, _data);
                }
            }
            else
            {
                Console.WriteLine("[ERROR] ConcreteMediator :: publishData :: Source = {0}, Destination = {0}!!", sourceName, destinationName);
            }
        }
    }
}
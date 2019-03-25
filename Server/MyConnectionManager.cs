using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;

namespace ChatSpace
{
    public class MyConnectionManager
    { 
        private static MyConnectionManager instance;

        private MyConnectionManager() {
        }

        public static MyConnectionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MyConnectionManager();
                }
                return instance;
            }
        }

        Dictionary<string, ConnectionContext> _tcpCo =
            new Dictionary<string, ConnectionContext>();
        Dictionary<string, string> _hubCo =
            new Dictionary<string, string>();

        public void AddHubConnection(string name, string id)
        {
            if (name.Equals(""))
            {
                name = "UserChat";
            }
            int i = 0;
            string tpmName = name;
            while (_hubCo.ContainsValue(name))
            {
                name = tpmName + i++;
            }
            while (_tcpCo.ContainsKey(name))
            {
                name = tpmName + i++;
            }
            _hubCo.Add(id, name);
        }

        public void AddTcpConnection(string name, ConnectionContext co )
        {
            if (name.Equals("")) {
                name = "UserChat";
            }
            int i = 0;
            string tpmName = name;
            while (_tcpCo.ContainsKey(name)){
                name = tpmName + i++;
            }
            while (_hubCo.ContainsValue(name))
            {
                name = tpmName + i++;
            }
            _tcpCo.Add(name, co);
        }

        public async Task SendMessageCo(string user, string messages)
        {
            for (var i = _tcpCo.Values.GetEnumerator(); i.MoveNext();)
            {
                ConnectionContext co = i.Current;
                await co.Transport.Output.WriteAsync(Encoding.ASCII.GetBytes("[" + user + "]" + messages + "\r\n"));
            }
        }

        public string GetName(ConnectionContext co)
        {
            var keys = from entry in _tcpCo
                       where entry.Value == co
                       select entry.Key;

            if (keys.Count<string>() > 0)
                return Regex.Replace(keys.ElementAt(0), @"\t|\n|\r", "");
            return "name";
        }

        public string GetName(string id)
        {

            if (_hubCo.ContainsKey(id))
                return Regex.Replace(_hubCo[id], @"\t|\n|\r", "");
            return "name";
        }

        public void RemoveTcpConnection(ConnectionContext co)
        {
            var keys = from entry in _tcpCo
                       where entry.Value == co
                       select entry.Key;
            if (keys.Count<string>() > 0)
                _tcpCo.Remove(keys.ElementAt(0));
        }

        public void RemoveHubConnection(string id)
        {
            if (_hubCo.ContainsKey(id))
                _hubCo.Remove(id);
        }

        public List<string> UserList()
        {
            List<string> list = new List<string>();

            list = _tcpCo.Keys.ToList();
            list.AddRange(_hubCo.Values.ToList());
            return list;
        }
    }
}

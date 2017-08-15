using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CTP.DataSynchronization;
using CTP.Util;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

using System.Threading;

namespace DataSynchronization
{
    class Program
    {
        public static void Main(string[] args)
        {
      
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Agent.Process();
        }
    }
}
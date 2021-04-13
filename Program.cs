using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ComputationalQuestion
{
    class Program
    {
       
        
        static void Main(string[] args)
        {
            Assignment ass = new Assignment();

            ass.createDB();
            //ass.exeDB();
            //ass.loanRecycle();
            //ass.printPaymentsTable();



        }

    }

    


}

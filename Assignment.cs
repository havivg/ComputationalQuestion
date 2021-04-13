using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ComputationalQuestion
{
    class Assignment
    {
        public List<Record> payments;
        public const double TOTAL_FUND = 36000;
        public const double INTEREST1 = (3.25+0.1) / 100;
        public const double INTEREST2 = 4.5 / 100;
        public const int NUMBER_OF_MONTHS1 = 36;
        public const int NUMBER_OF_MONTHS2 = 48;
        private string connString;

        public Assignment()
        {
            this.payments = new List<Record>();
            connString = @"Server =.\SQLEXPRESS; Trusted_Connection = True;";
        }

        public void exeDB() {

            connString +=" Database = SpitzerTableDB;";
            //string connString = @"Server =.\SQLEXPRESS; Database = SpitzerTable; Trusted_Connection = True;";
            //string connString = @"Server =.\SQLEXPRESS; Trusted_Connection = True;";

            try
            {
                SqlCommand cmd;
                //sql connection object
                using (SqlConnection conn = new SqlConnection(connString))
                {
                       

                    //set stored procedure name
                    string spName = @"dbo.[createSpitzerTable]";

                    //define the SqlCommand object
                     cmd= new SqlCommand(spName, conn);

                    //Set SqlParameter 
                    SqlParameter param1 = new SqlParameter();
                    param1.ParameterName = "@total";
                    param1.SqlDbType = SqlDbType.Float;
                    param1.Value = TOTAL_FUND;

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@numberOfPayment";
                    param2.SqlDbType = SqlDbType.Int;
                    param2.Value = NUMBER_OF_MONTHS1;

                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@interest";
                    param3.SqlDbType = SqlDbType.Float;
                    param3.Value = INTEREST1;

                    SqlParameter param4 = new SqlParameter();
                    param4.ParameterName = "@month";
                    param4.SqlDbType = SqlDbType.Int;
                    param4.Value = 12;

                    SqlParameter param5 = new SqlParameter();
                    param5.ParameterName = "@payment";
                    param5.SqlDbType = SqlDbType.Float;
                    param5.Value = calcTotalPayment(TOTAL_FUND,INTEREST1, NUMBER_OF_MONTHS1);

                    //add the parameter to the SqlCommand object
                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
                    cmd.Parameters.Add(param4);
                    cmd.Parameters.Add(param5);

                    //open connection
                    conn.Open();

                    //set the SqlCommand type to stored procedure and execute
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader dr = cmd.ExecuteReader();


                    int month;
                    double open, interest, fund, total, close;
                    //check if there are records
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {

                            month = dr.GetInt32(0);
                            open = dr.GetDouble(1);
                            interest = dr.GetDouble(2);
                            fund = dr.GetDouble(3);
                            total = dr.GetDouble(4);
                            close = dr.GetDouble(5);

                            //add record to payments list
                            payments.Add(new Record(month, open, interest, fund, total, close));

                        }
                    }
                    else
                    {
                        Console.WriteLine("No data found.");
                    }

                    //close data reader
                    dr.Close();

                    //close connection
                    conn.Close();

                }
            }
            catch (Exception ex)
            {
                //display error message
                Console.WriteLine("Exception: " + ex.Message);
            }

        }

        public void createDB()
        {
            //string connString = @"Server =.\SQLEXPRESS; Trusted_Connection = True;";

            try
            {
                SqlCommand cmd;
                //sql connection object
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    //create db
                    var command = conn.CreateCommand();
                    command.CommandText = "CREATE DATABASE SpitzerTableDB";
                    command.ExecuteNonQuery();
                    conn.Close();
                }
                connString = @"Server =.\SQLEXPRESS; Database = SpitzerTableDB; Trusted_Connection = True;";
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    //create table for spitzer table
                    string createTbl = "CREATE TABLE spitzerTbl" +
                    "(Month INTEGER PRIMARY KEY," +
                     "BalanceOpenFund FLOAT, InterestPayment FLOAT, FundPayment FLOAT," +
                     "TotalPayment FLOAT, BalanceCloseFund FLOAT)";
                    cmd = new SqlCommand(createTbl, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public void loanRecycle()
        {
            
            //start loan recycle from month 12 close fund balance
            double fund =payments[11].closeFundBalance;
            
            //calculate the total payment for each month
            double payment = calcTotalPayment(fund , INTEREST2 ,NUMBER_OF_MONTHS2);
            double interestPay, fundPay, closeFund;

            for (int i = 0; i < NUMBER_OF_MONTHS2; i++)
            {
                interestPay = fund * INTEREST2;
                fundPay = payment - interestPay;
                closeFund = fund - fundPay;
                payments.Add(new Record(i + 13, fund, interestPay, fundPay, payment, closeFund));
                fund = closeFund;
            }
        }


        //caculate the const total payment per month
        public double calcTotalPayment(double total, double interest, double numOfMonth)
        {
            return (total * interest * Math.Pow((1 + interest), numOfMonth)) / (Math.Pow((1 + interest), numOfMonth) - 1);
        }


        //print all the record in payments list
        public void printPaymentsTable()
        {
            Console.WriteLine("Month\tOpen Fund Balance\tInterest Payment\tFund Payment\tTotalPayment\tClose Fund Balance");
                    foreach (Record record in payments)
            {
                Console.WriteLine(record.ToString());
            }
        }
    }

    class Record
    {
        public int month { get; set; }
        public double openFundBalance { get; set; }
        public double interestPayment { get; set; }
        public double fundPayment { get; set; }
        public double totalPayment { get; set; }
        public double closeFundBalance { get; set; }

        public Record(int month, double openFundBalance, double interestPayment, double fundPayment,
            double totalPayment, double closeFundBalance)
        {
            this.month = month;
            this.openFundBalance = openFundBalance;
            this.interestPayment = interestPayment;
            this.fundPayment = fundPayment;
            this.totalPayment = totalPayment;
            this.closeFundBalance = closeFundBalance;
        }

        public override string ToString()
        {
            return month.ToString() + "\t\t" + String.Format("{0:0.00}", openFundBalance) + "\t\t" + String.Format("{0:0.00}", interestPayment) + "\t\t" +
                String.Format("{0:0.00}", fundPayment) + "\t\t" + String.Format("{0:0.00}", totalPayment) + "\t\t" + String.Format("{0:0.00}", closeFundBalance);
        }


    }

}

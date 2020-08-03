using ConnTest;


class Program
{
    static void Main(string[] args)
    {
        string connStr = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.0.1)(PORT=8080)))(CONNECT_DATA=(SID=sidname)(SERVER=DEDICATED))); User Id = login; Password = pass;";

        OracleRepo oracleRepo = new OracleRepo(connStr);

        oracleRepo.CheckConnection();

        oracleRepo.GetSomeThingResult();

    }
}


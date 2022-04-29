namespace Backend {
    public class DatabaseProperties{
        public string databaseUrl;
        public int portNumber;
        public string databaseName;
        public string username;
        public string password;

        public DatabaseProperties(
            string databaseUrl = "cloud-vm-42-36.doc.ic.ac.uk",
            int portNumber = 7474,
            string databaseName = "neo4j",
            string username = "neo4j",
            string password = "s3cr3t"
            ) 
        {

            this.databaseUrl = databaseUrl;
            this.portNumber = portNumber;
            this.databaseName = databaseName;
            this.username = username;
            this.password = password;
        }


    }
}
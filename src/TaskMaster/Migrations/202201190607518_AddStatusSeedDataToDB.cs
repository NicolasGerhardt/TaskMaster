namespace TaskMaster.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusSeedDataToDB : DbMigration
    {
        public override void Up()
        {
            Sql("insert into status (Name) values ('To Do');");
            Sql("insert into status (Name) values ('In Progress');");
            Sql("insert into status (Name) values ('Done');");
        }
        
        public override void Down()
        {
            //TODO add removal of Inserted Values
        }
    }
}

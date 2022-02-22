namespace Universe.SharePoint.DataAccess.Test.Models
{
    using Sp.DataAccess.Models;

    public class TrainsetSp : EntitySp
    {
        public override string ListUrl => "Lists/Trainset";

        public string Name { get; set; }

        public string Title { get; set; }
    }
}
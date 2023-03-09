namespace InfoService.Repositories
{
    public class SiteProvider : BaseProvider
    {
        DriverRespository driver;
        public DriverRespository Driver
        {
            get
            {
                if (driver == null)
                {
                    driver = new DriverRespository(Context);
                }
                return driver;
            }
        }
    }
}

namespace Assets._Script
{
    public class DependencyService : Singleton<DependencyService>
    {
        /*private DistrictController districtController;
        public DistrictController District()
        {
            if (districtController == null)
            {
                districtController = FindObjectOfType<DistrictController>();
            }
            return districtController;
        }*/

        private GameManager gameManager;
        public GameManager Game()
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
            return gameManager;
        }
    }
}

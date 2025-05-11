namespace DateApp.Core.utils
{
    public static class LocationUtils
    {
        //yaklaşık 30-33 km 
        private const double GridCellSizeDegrees = 0.3;

        // Enlem/boylam değerine göre grid hücre ID'si üretir.
        // Aynı hücredeki kullanıcılar “yakın” kabul edilir.
        public static string GetGridCellId(double latitude, double longitude)
        {
            // Negatif değerler için => Math.Floor
            int latCell = (int)Math.Floor(latitude / GridCellSizeDegrees);
            int lonCell = (int)Math.Floor(longitude / GridCellSizeDegrees);
            return $"Cell_{latCell}_{lonCell}";
        }
        //Haversine formülü 
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Dünya yarıçapı (km)
            var latRad1 = ToRadians(lat1);
            var lonRad1 = ToRadians(lon1);
            var latRad2 = ToRadians(lat2);
            var lonRad2 = ToRadians(lon2);

            var deltaLat = latRad2 - latRad1;
            var deltaLon = lonRad2 - lonRad1;

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(latRad1) * Math.Cos(latRad2) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        private static double ToRadians(double angle) => Math.PI * angle / 180.0;

    }
}
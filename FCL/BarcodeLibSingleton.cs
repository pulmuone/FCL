
namespace FCL
{
    public class BarcodeLibSingleton
    {
        static readonly BarcodeLibSingleton instance = new BarcodeLibSingleton();

        private BarcodeLib.Barcode barcode;

        private BarcodeLibSingleton()
        {
            barcode = new BarcodeLib.Barcode();
        }

        public static BarcodeLibSingleton Instance
        {
            get
            {
                return instance;
            }
        }

        public BarcodeLib.Barcode Barcode 
        {
            get
            {
                return barcode;
            }
        }
    }
}

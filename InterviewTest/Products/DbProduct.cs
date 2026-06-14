namespace InterviewTest.Products
{
    public class DbProduct : IProduct
    {
        private readonly string _productNumber;
        private readonly float _sellingPrice;

        public DbProduct(string productNumber, float sellingPrice)
        {
            _productNumber = productNumber;
            _sellingPrice = sellingPrice;
        }

        public string GetProductNumber() => _productNumber;
        public float GetSellingPrice() => _sellingPrice;
    }
}

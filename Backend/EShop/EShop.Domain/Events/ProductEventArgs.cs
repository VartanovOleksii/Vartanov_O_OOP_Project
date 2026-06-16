namespace EShop.Domain.Events;

public class ProductEventArgs : EventArgs
{
    public int ProductId { get; }

    public int OldQuantity { get; }

    public int NewQuantity { get; }

    public ProductEventArgs(int productId, int oldQuantity, int newQuantity)
    {
        ProductId = productId;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
    }
}
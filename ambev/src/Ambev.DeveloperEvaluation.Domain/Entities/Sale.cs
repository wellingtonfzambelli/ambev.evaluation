using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public sealed class Sale : BaseEntity
{
    public string SaleNumber { get; private set; }
    public DateTime SaleDate { get; private set; }
    public readonly DateTime CreatedAt;
    public DateTime? UpdateAt { get; private set; }
    public SaleStatus SaleStatus { get; private set; }
    public decimal TotalAmount => _items.Sum(i => i.Total);
    public Guid CustomerId { get; private set; }
    public Guid BranchId { get; private set; }
    public IReadOnlyCollection<SaleItem> Items => _items;
    private readonly List<SaleItem> _items = new();

    public User User { get; private set; }
    public Branch Branch { get; private set; }

    public Sale
    (
        string saleNumber,
        DateTime saleDate,
        Guid customerId,
        Guid branchId
    )
    {
        SaleNumber = saleNumber;
        SaleDate = saleDate;
        CustomerId = customerId;
        BranchId = branchId;
        SaleStatus = SaleStatus.NotCanceled;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (_items.FirstOrDefault(i => i.ProductId == productId)
            is var existingItem && existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            UpdateAt = DateTime.UtcNow;

            return;
        }

        _items.Add(new SaleItem(productId, quantity, unitPrice));
        UpdateAt = DateTime.UtcNow;
    }

    public void CancelStatus()
    {
        if (SaleStatus == SaleStatus.Canceled)
            return;

        SaleStatus = SaleStatus.Canceled;
        UpdateAt = DateTime.UtcNow;
    }
}
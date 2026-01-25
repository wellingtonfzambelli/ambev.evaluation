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
    public string CustomerName { get; private set; } = string.Empty;
    public Guid BranchId { get; private set; }
    public string BranchName { get; private set; } = string.Empty;
    public IReadOnlyCollection<SaleItem> Items => _items;
    private readonly List<SaleItem> _items = new();

    public User User { get; private set; }
    public Branch Branch { get; private set; }

    public Sale
    (
        string saleNumber,
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName
    )
    {
        SaleNumber = saleNumber;
        SaleDate = saleDate;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        SaleStatus = SaleStatus.NotCanceled;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (_items.FirstOrDefault(i => i.ProductId == productId)
            is var existingItem && existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            UpdateAt = DateTime.UtcNow;

            return;
        }

        _items.Add(new SaleItem(productId, productName, quantity, unitPrice));
        UpdateAt = DateTime.UtcNow;
    }

    public void ReplaceItems(IEnumerable<(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        _items.Clear();

        foreach (var item in items)
        {
            _items.Add(new SaleItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice));
        }

        UpdateAt = DateTime.UtcNow;
    }

    public bool CancelStatus()
    {
        if (SaleStatus == SaleStatus.Canceled)
            return false;

        SaleStatus = SaleStatus.Canceled;
        UpdateAt = DateTime.UtcNow;
        return true;
    }
}
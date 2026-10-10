using System;
using System.Linq;
using System.Collections.Generic;

class Program {
    static void Main() {
        var batches = new List<Batch> {
            new Batch { Id = Guid.NewGuid(), BatchNumber = "P5-2026-001", CurrentStock = 100m },
            new Batch { Id = Guid.NewGuid(), BatchNumber = "P5-2026-002", CurrentStock = 0m }
        };
        
        var query = batches.Select(b => new { id = b.Id, batchNumber = b.BatchNumber, currentStock = b.CurrentStock });
        var json = System.Text.Json.JsonSerializer.Serialize(query);
        Console.WriteLine(json);
    }
}
class Batch {
    public Guid Id { get; set; }
    public string BatchNumber { get; set; }
    public decimal CurrentStock { get; set; }
}

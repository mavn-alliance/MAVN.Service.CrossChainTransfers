using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAVN.Service.CrossChainTransfers.MsSqlRepositories.Entities
{
    [Table("deduplication_log")]
    public class DeduplicationEntity
    {
        [Key]
        [Column("deduplication_id")]
        public string DeduplicationId { get; set; }

        [Column("Timestamp")]
        public DateTime Timestamp { get; set; }

        public static DeduplicationEntity Create(string deduplicationId, DateTime timestamp)
        {
            return new DeduplicationEntity
            {
                DeduplicationId = deduplicationId,
                Timestamp = timestamp
            };
        }
    }
}

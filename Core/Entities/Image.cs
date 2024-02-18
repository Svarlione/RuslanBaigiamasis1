using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RuslanAPI.Core.Models
{
    public class Image
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public byte[] ImageBytes { get; set; } = null!;

        public string Description { get; set; } = null!;

        [NotMapped] // Помечаем свойство как не отображаемое в базе данных
        public int Size => ImageBytes?.Length ?? 0; // Автоматически вычисляемый размер из массива байт


        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
    }
}

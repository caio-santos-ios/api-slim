using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class PermissionProfile : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Mesma estrutura de módulos/rotinas já usada em User.Modules.
        /// Ao aplicar em um usuário, estes módulos são copiados como base
        /// e podem ser ajustados individualmente depois.
        /// </summary>
        [BsonElement("modules")]
        public List<api_slim.src.Models.Module> Modules { get; set; } = [];
    }
}

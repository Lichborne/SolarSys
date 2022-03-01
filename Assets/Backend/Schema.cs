using System.ComponentModel.DataAnnotations;

namespace Backend
{
    public class NodeCreationSchema
    {
        [Required]
        public string Title;

        [Required]
        public string Description;

        [Required]
        public Guid Id;

        [Required]
        public (double X, double Y, double Z) Coordinates;
    }

    public class ValuePair<T>
    {
        [Required]
        T NewValue;

        [Required]
        T OldValue;
    }

    public class NodeUpdateSchema
    {
        ValuePair<String> Title;

        ValuePair<String> Body;

        [Required]
        Guid Id;

        ValuePair<(double X, double Y, double Z)> Coordinates;
    }

    public class EdgeCreationSchema
    {
        [Required]
        string Title;

        [Required]
        string Body;

        [Required]
        Guid Id;

        [Required]
        Guid ParentId;

        [Required]
        Guid ChildId;
    }

    public class EdgeUpdateSchema
    {
        [Required]
        Guid Id;

        ValuePair<string> Title;

        ValuePair<string> Body;

        ValuePair<Guid> ParentId;

        ValuePair<Guid> ChildId;
    }

    public class NodeDeletionSchema
    {


    }
}
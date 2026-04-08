using System.Collections.Generic;
using Winslop;

public class FeatureNode
{
    public string Name { get; set; }
    public bool IsCategory => Feature == null;
    public FeatureBase Feature { get; }
    public List<FeatureNode> Children { get; set; } = new List<FeatureNode>();


   // Property to control default checked state
   public bool DefaultChecked { get; set; } = true;

    // Constructor for categories
    public FeatureNode(string name)
    {
        Name = name;
    }

    // Constructor for actual features
    public FeatureNode(FeatureBase feature)
    {
        Feature = feature;
        Name = feature.ID(); // Use the ID as the name
    }
}

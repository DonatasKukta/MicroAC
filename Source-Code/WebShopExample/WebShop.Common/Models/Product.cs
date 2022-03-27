namespace WebShop.Common
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = LoremIpsum;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 0;
        //TODO: Image

        const string LoremIpsum = @"
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris facilisis laoreet dui, 
vitae mollis lacus vestibulum nec. Nunc id lorem eget elit eleifend hendrerit. Sed eget 
risus ultricies, elementum augue quis, hendrerit ipsum. Integer scelerisque malesuada mauris, 
vitae ullamcorper libero. Aliquam malesuada pellentesque mauris eu euismod. Praesent accumsan 
lorem non commodo efficitur. Nullam bibendum quam in lectus accumsan, et auctor nunc laoreet. 
Suspendisse porta dolor nisi, sed volutpat ligula pellentesque in. Vestibulum scelerisque sodales 
semper. Donec eros ante, feugiat sed fringilla vitae, lacinia nec nibh. Etiam enim risus, venenatis 
et mauris sit amet, porttitor rutrum tellus. Integer convallis quis risus sit amet lobortis. 
Suspendisse tristique sollicitudin tellus et congue. Vivamus feugiat ligula mi, non molestie 
odio posuere a.";
    }

}

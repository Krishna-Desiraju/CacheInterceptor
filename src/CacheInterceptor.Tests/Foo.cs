namespace CacheInterceptor.Tests
{
    using System;

    /// <summary>
    /// The foo.
    /// </summary>
    public class Foo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Foo"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public Foo(string value)
        {
            this.Value1 = Guid.NewGuid().ToString();
            this.Value2 = value;
        }

        /// <summary>
        /// Gets or sets the value 1.
        /// </summary>
        public string Value1 { get; set; }

        /// <summary>
        /// Gets or sets the value 2.
        /// </summary>
        public string Value2 { get; set; }
    }
}
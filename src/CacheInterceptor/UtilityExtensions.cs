namespace CacheInterceptor
{
    using System.IO;
    using System.Runtime.Serialization;

    /// <summary>
    /// The serialization extensions.
    /// </summary>
    public static class UtilityExtensions
    {
        /// <summary>
        /// The data contract serialize to xml string.
        /// </summary>
        /// <param name="objectToSerialize">
        /// The object to serialize.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DataContractSerializeToXmlString(this object objectToSerialize)
        {
            using (var memStm = new MemoryStream())
            {
                var serializer = new DataContractSerializer(objectToSerialize.GetType());
                serializer.WriteObject(memStm, objectToSerialize);

                memStm.Seek(0, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(memStm))
                {
                    string result = streamReader.ReadToEnd();
                    return result;
                }
            }
        }

        /// <summary>
        /// The dump object.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DumpObject(this object argument)
        {
            if (argument == null)
            {
                return "null";
            }

            var objtype = argument.GetType();

            if (objtype == typeof(string) || objtype.IsPrimitive || !objtype.IsClass)
            {
                return "'" + argument + "'";
            }

            return argument.DataContractSerializeToXmlString();
        }
    }
}

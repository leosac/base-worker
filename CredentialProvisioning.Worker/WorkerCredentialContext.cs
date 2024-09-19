using Leosac.CredentialProvisioning.Core.Contexts;
using Leosac.CredentialProvisioning.Core.Models;

namespace Leosac.CredentialProvisioning.Worker
{
    /// <summary>
    /// The base worker credential context.
    /// </summary>
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    /// <param name="deviceContext">The associated device context.</param>
    /// <param name="credential">The associated credential details.</param>
    public class WorkerCredentialContext(DeviceContext deviceContext, WorkerCredentialBase? credential = null)
    {
        /// <summary>
        /// The associated device context.
        /// </summary>
        public DeviceContext DeviceContext { get; private set; } = deviceContext;

        /// <summary>
        /// The associated credential details.
        /// </summary>
        public WorkerCredentialBase? Credential { get; private set; } = credential;


        /// <summary>
        /// List of fields values changed since the initial credential details assignment.
        /// </summary>
        public IList<string> FieldsChanged { get; private set; } = [];

        /// <summary>
        /// Update a field value from the credential details.
        /// </summary>
        /// <param name="fieldName">The targeted field name.</param>
        /// <param name="fieldValue">The new field value.</param>
        public void UpdateFieldValue(string fieldName, object? fieldValue)
        {
#pragma warning disable IDE0019 // Use pattern matching
            var data = Credential?.Data as IDictionary<string, object>;
#pragma warning restore IDE0019 // Use pattern matching
            if (data != null && data.TryGetValue(fieldName, out object? value))
            {
                if (fieldValue != null)
                {
                    if (fieldValue is byte[] bv)
                    {
#if NET48
                        fieldValue = Core.ConvertBackport.ToHexString(bv);
#else
                        fieldValue = Convert.ToHexString(bv);
#endif
                    }

                    if (value?.ToString() != fieldValue.ToString())
                    {
                        data[fieldName] = fieldValue;
                        if (!FieldsChanged.Contains(fieldName))
                        {
                            FieldsChanged.Add(fieldName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get a field value from the credential details.
        /// </summary>
        /// <param name="fieldName">The targeted field name.</param>
        /// <returns>The current field value.</returns>
        public object? GetFieldValue(string fieldName)
        {
#pragma warning disable IDE0019 // Use pattern matching
            var data = Credential?.Data as IDictionary<string, object>;
#pragma warning restore IDE0019 // Use pattern matching
            if (data != null && data.TryGetValue(fieldName, out object? value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Get a field value from the credential details as a byte array.
        /// </summary>
        /// <param name="fieldName">The targeted field name.</param>
        /// <returns>The current binary field value.</returns>
        /// <remarks>
        /// The following converting logic will be applied according to the real field value type:
        ///  - Byte array will be returned directly
        ///  - String will be converted as HexString (eg. "aabbcc" => [ 0xaa, 0xbb, 0xcc ])
        ///  - Numeric will be converted to their binary representation (eg. 10000 => [ 0x10, 0x27 ] if the server is Little Endian arch, [0x27, 0x10] otherwise);
        /// </remarks>
        public byte[]? GetBinaryFieldValue(string fieldName)
        {
            byte[]? ret = null;
            var v = GetFieldValue(fieldName);
            if (v != null)
            {
                if (v is byte[] bv)
                {
                    ret = bv;
                }
                else if (v is long slv)
                {
                    ret = BitConverter.GetBytes(slv);
                }
                else if (v is ulong ulv)
                {
                    ret = BitConverter.GetBytes(ulv);
                }
                else
                {
#if NET48
                    ret = Core.ConvertBackport.FromHexString(v.ToString()!);
#else
                    ret = Convert.FromHexString(v.ToString()!);
#endif
                }
            }
            return ret;
        }
    }
}

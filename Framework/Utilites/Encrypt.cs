using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Encryption;
public class EncryptionService
{
    private readonly IJSRuntime _jsRuntime;

    public EncryptionService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> EncryptString(string plainText, string secretKey)
    {
        Console.WriteLine("Encrypt in Encrypt.cs called");
        return await _jsRuntime.InvokeAsync<string>("encryptString", plainText, secretKey);
    }

    public async Task<string> DecryptString(string encryptedText, string secretKey)
    {
        return await _jsRuntime.InvokeAsync<string>("encrypt.decryptString", encryptedText, secretKey);
    }
}

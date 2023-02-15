using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace NetNuxt.Controllers;

public class BaseController : Controller
{
    #region Call API

    private static readonly HttpClient client = httpClient();

    private static HttpClient httpClient()
    {
        HttpClientHandler handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
        };
        HttpClient httpClient = new HttpClient(handler);
        //表头参数
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        httpClient.Timeout = TimeSpan.FromSeconds(600);

        return httpClient;
    }

    protected static string PostRequest(string url, string data)
    {
        HttpContent httpContent = new StringContent(data);
        var task = client.PostAsync(url, httpContent);
        task.Wait();
        if (task.Result.IsSuccessStatusCode)
        {
            var read = task.Result.Content.ReadAsStringAsync();
            read.Wait();
            return read.Result;
        }

        return "";
    }

    #endregion Call API

    #region 加密解密

    /// <summary>
    /// 128位处理key
    /// </summary>
    /// <param name="keyArray">原字节</param>
    /// <param name="key">处理key</param>
    /// <returns></returns>
    private static byte[] GetAesKey(IReadOnlyList<byte> keyArray, string key)
    {
        byte[] newArray = new byte[16];
        //if (keyArray.Length < 16)
        //{
        for (int i = 0; i < newArray.Length; i++)
        {
            if (i >= keyArray.Count)
            {
                newArray[i] = 0;
            }
            else
            {
                newArray[i] = keyArray[i];
            }
        }

        //}
        return newArray;
    }

    /// <summary>
    /// 使用AES加密字符串,按128位处理key
    /// </summary>
    /// <param name="content">加密内容</param>
    /// <param name="key">秘钥，需要128位、256位.....</param>
    /// <param name="autoHandle"></param>
    /// <returns>Base64字符串结果</returns>
    protected static string AesEncrypt(string content, string key, bool autoHandle = true)
    {
        byte[] keyArray = Encoding.UTF8.GetBytes(key);
        if (autoHandle)
        {
            keyArray = GetAesKey(keyArray, key);
        }

        byte[] toEncryptArray = Encoding.UTF8.GetBytes(content);

        SymmetricAlgorithm des = Aes.Create();
        des.Key = keyArray;
        des.Mode = CipherMode.ECB;
        des.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = des.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray);
    }

    /// <summary>
    /// 使用AES解密字符串,按128位处理key
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="key">秘钥，需要128位、256位.....</param>
    /// <param name="autoHandle"></param>
    /// <returns>UTF8解密结果</returns>
    protected static string AesDecrypt(string content, string key, bool autoHandle = true)
    {
        byte[] keyArray = Encoding.UTF8.GetBytes(key);
        if (autoHandle)
        {
            keyArray = GetAesKey(keyArray, key);
        }

        byte[] toEncryptArray = Convert.FromBase64String(content);

        SymmetricAlgorithm des = Aes.Create();
        des.Key = keyArray;
        des.Mode = CipherMode.ECB;
        des.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = des.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return Encoding.UTF8.GetString(resultArray);
    }

    protected static string MD5Encrypt(string content)
    {
        var md5_byte = MD5.HashData(Encoding.UTF8.GetBytes(content));
        var md5_str = BitConverter.ToString(md5_byte).ToLower();
        return md5_str.Replace("-", "");
    }

    #endregion 加密解密
}
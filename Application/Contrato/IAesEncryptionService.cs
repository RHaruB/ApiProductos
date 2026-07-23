using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contrato
{
    public interface IAesEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}

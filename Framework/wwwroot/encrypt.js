window.encryption = {
    encryptString: function (plainText, secretKey) {
        console.log('encryptString in encrypt.js called');
        return CryptoJS.AES.encrypt(plainText, secretKey).toString();
    },
    decryptString: function (encryptedText, secretKey) {
        var bytes = CryptoJS.AES.decrypt(encryptedText, secretKey);
        return bytes.toString(CryptoJS.enc.Utf8);
    }
};

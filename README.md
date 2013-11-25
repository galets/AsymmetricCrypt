AsymmetricCrypt
===============

Simple .NET asymmetric encryption implementation

Purpose:
-------------------------

data encryption on the systems, where storing password or key in cleartext is not desirable option.


Usage:
-------------------------

1. Generate private key:

    `AsymmetricCrypt --genkey >private.key`

2. Extract public key component from private key:

    `AsymmetricCrypt --publickey <private.key >public.key`

3. Encrypt file using public key:   

    `AsymmetricCrypt --encrypt public.key <plaintext.txt >encrypted.ascr`
   
4. Decrypt file using private key:

    `AsymmetricCrypt --decrypt private.key <encrypted.ascr >plaintext.txt`


Internals:
-------------------------

Each file is encrypted using AES256 with randomly generated key. AES256 key is encrypted using 4096
bit RSA and stored with the file.

File structure:

    4 bytes:    signature  "ASCR"
    16 bytes:   AES IV
    512 bytes:  RSA 4096-encrypted AES key
    rest:       encrypted file contents


Installing on Ubuntu:
-------------------------

Must install prerequisites using following commands:

    sudo apt-get install mono-runtime libmono-system2.0-cil


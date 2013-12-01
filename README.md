AsymmetricCrypt
===============

Simple .NET asymmetric encryption implementation

Purpose:
-------------------------

data encryption on the systems, where storing password or key in cleartext is not desirable option.


Usage:
-------------------------

Tool will use standard io streams where possible, or files could be specified on command line

Generate private key:

	AsymmetricCrypt --genkey private.key
	AsymmetricCrypt --genkey >private.key

Extract public key component from private key:

	AsymmetricCrypt --publickey private.key public.key
	AsymmetricCrypt --publickey <private.key >public.key

Encrypt file using public key:   

	AsymmetricCrypt --encrypt public.key plaintext.txt encrypted.ascr
	AsymmetricCrypt --encrypt public.key <plaintext.txt >encrypted.ascr
   
Decrypt file using private key:

	AsymmetricCrypt --decrypt private.key encrypted.ascr plaintext.txt
	AsymmetricCrypt --decrypt private.key <encrypted.ascr >plaintext.txt


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

Install using following commands:

	sudo apt-get install mono-runtime libmono-system2.0-cil wget
	sudo mkdir -p /usr/share/ascry
	sudo wget https://raw.github.com/galets/AsymmetricCrypt/master/Binary/AsymmetricCrypt.exe -O /usr/share/ascry/AsymmetricCrypt.exe
	sudo chmod +x /usr/share/ascry/AsymmetricCrypt.exe
	sudo ln -s /usr/share/ascry/AsymmetricCrypt.exe /usr/bin/ascry




pushd %0\..\bin\release  || goto error

AsymmetricCrypt --genkey private1.key || goto error
AsymmetricCrypt --genkey >private2.key || goto error

AsymmetricCrypt --publickey private1.key public1.key || goto error
AsymmetricCrypt --publickey <private2.key >public2.key || goto error

copy AsymmetricCrypt.pdb plaintext.txt
AsymmetricCrypt --encrypt public1.key plaintext.txt encrypted1.ascr || goto error
AsymmetricCrypt --encrypt public2.key <plaintext.txt >encrypted2.ascr || goto error
   
AsymmetricCrypt --decrypt private1.key encrypted1.ascr plaintext1.txt || goto error
AsymmetricCrypt --decrypt private2.key <encrypted2.ascr >plaintext2.txt || goto error

fc /b plaintext.txt plaintext1.txt || goto error
fc /b plaintext.txt plaintext2.txt || goto error
goto end

:error
echo ****** ERROR ************
popd
exit /b 1

:end
popd
# HawkEye config decryptor

How to decrypt config of the HawkEye Keylogger - Reborn v9:

  - unpack Reborn Stub.exe from the malware sample
    - in can be packed with ConfuserEx v1.0.0, SmartAssembly,...
  - extract resource RCData (e.g. with ResourceHacker),
    - convert to base64 and add to the source code of this decryptr
  - decompile Reborn Stub.exe and find the class only with one-two methods and static string similar to the password
    - change the password in the source code of this decryptor
  - build and run decryptor
  - tested with the HawkEye Keylogger - Reborn v9, Version=9.0.0.5

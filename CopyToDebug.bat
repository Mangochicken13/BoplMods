echo Clearing %3
del /s /q %3

echo Copying output to %3
copy /y %1\%2.dll %3
copy /y %1\%2.pdb %3
del %3\%2.dll.mdb
"C:\Users\mango\Documents\Bopl\pdb2mdb\pdb2mdb.exe" %3\%2.dll
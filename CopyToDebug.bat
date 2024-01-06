echo Clearing %3
del /s /q %3

echo Copying output to %3
copy /y %1\%2.dll %3
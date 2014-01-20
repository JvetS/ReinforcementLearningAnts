@echo off
FOR /l %%e IN (1,1,200) DO (
%~dp0..\Python\App\python.exe "%~dp0playgame.py" ^
 --engine_seed 42 ^
 --player_seed 42 ^
 --end_wait=0.5 ^
 --verbose ^
 -E -e ^
 --log_dir game_logs ^
 --turntime=20000000 ^
 --loadtime=1000000 ^
 --turns 10 ^
 --nolaunch ^
 --map_file "%~dp0maps\practical2\corridor.map" ^
 "%~dp0..\Practical2Bot\bin\Debug\MyBot.exe" ^
 "%~dp0..\Python\App\python.exe ""%~dp0sample_bots\dommebot\MyBot.py3""" )

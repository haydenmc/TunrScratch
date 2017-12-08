@echo off
REM To address gulp stupidity (https://github.com/gulpjs/gulp/issues/259)
REM This will run gulp build, then gulp watch in a loop
cmd /c gulp build
:watch
cmd /c gulp watch
timeout 1
goto watch

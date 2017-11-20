var gulp = require('gulp');
var htmlImport = require('gulp-html-import');
var webpack = require('webpack');
var gulpWebpack = require('webpack-stream');

gulp.task('build-assets', function() {
    gulp.src('./Client/Assets/**/*')
        .pipe(gulp.dest('./wwwroot/Assets'));
});

gulp.task('build-scripts', function () {
    return gulp.src('Client/Scripts/Tunr.ts')
        .pipe(gulpWebpack({
            module: {
                rules: [
                    {
                    test: /\.tsx?$/,
                    use: 'ts-loader',
                    exclude: /node_modules/
                    }
                ]
            },
            resolve: {
                extensions: [ '.tsx', '.ts', '.js' ]
            },
            output: {
                filename: 'tunr.js'
            }
        }, webpack))
        .pipe(gulp.dest('./wwwroot/Scripts/'));
});

gulp.task('build-html', function() {
    gulp.src('./Client/Views/index.html')
        .pipe(htmlImport('./Client/Views/'))
        .pipe(gulp.dest('./wwwroot'));
});

gulp.task('build', ['build-assets', 'build-scripts', 'build-html']);

gulp.task('watch', function() {
    gulp.watch('./Client/Assets/**/*', ['build-assets']);
    gulp.watch('./Client/Scripts/**/*', ['build-scripts']);
    gulp.watch('./Client/Views/**/*', ['build-html']);
});
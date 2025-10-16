// karma.conf.js
   module.exports = function (config) {
     config.set({
       basePath: '',
       frameworks: ['jasmine', '@angular-devkit/build-angular'],
       plugins: [
         require('karma-jasmine'),
         require('karma-chrome-launcher'),
         require('karma-jasmine-html-reporter'),
         require('karma-coverage'),
         require('@angular-devkit/build-angular/plugins/karma')
       ],
       client: {
         jasmine: {
           // Puedes agregar configuraciones de jasmine aquí.
         },
         clearContext: false // Deja el contexto de resultados en el navegador.
       },
       jasmineHtmlReporter: {
         suppressAll: true // elimina mensajes duplicados en la consola.
       },
       coverageReporter: {
         dir: require('path').join(__dirname, './coverage'),
         subdir: '.',
         reporters: [
           { type: 'html' },
           { type: 'text-summary' }
         ]
       },
       reporters: ['progress', 'kjhtml'],
       port: 9876,
       colors: true,
       logLevel: config.LOG_INFO,
       autoWatch: true,
       browsers: ['Chrome'],
       singleRun: false,
       restartOnFileChange: true
     });
   };

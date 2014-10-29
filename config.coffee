exports.config =
  
  paths:
    public: 'public'
  
  files:
    javascripts:
      defaultExtension: 'js'
      joinTo:
        'js/app.js': /^app/
        'js/vendor.js': /^(vendor|bower_components)/
      order:
        after: [
          # popover requires tooltip
          'vendor/js/bootstrap/tooltip.js'
          'vendor/js/bootstrap/popover.js'
          
          # collapse requires transitions
          'vendor/js/bootstrap/collapse.js'
          'vendor/js/bootstrap/transition.js'
          
        ]
         
    stylesheets:
      defaultExtension: 'less'
      joinTo: 
        'css/app.css': /^(app|vendor)/

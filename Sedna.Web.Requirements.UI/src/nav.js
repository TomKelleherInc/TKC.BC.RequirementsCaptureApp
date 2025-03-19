export default {
    items: [

      {
        name: 'Requirements Capture',
        url: '/collectors/pdf',
        icon: 'fa fa-cogs',
    },

      {
        divider: true
      },

      {
        name: 'Administration',
        url: '/components',
        icon: 'fa fa-puzzle-piece',
        children: [
          {
            name: 'Entity Types',
            url: '/components/entities',
            icon: 'fa fa-puzzle-piece'            
          },
          {
            name: 'Topics',
            url: '/admin/admin',
            icon: 'fa fa-id-badge'
          },
          {
            name: 'Source Types',
            url: '/components/sourceTypes',
            icon: 'fa fa-bars'
          },
          {
            name: 'Topics',
            url: '/components/sourceTypes',
            icon: 'fa fa-bars'
          },
          {
            name: 'NAV ITMs',
            url: '/components/sourceTypes',
            icon: 'fa fa-bars'
          }
        ]
      }
    ]
}
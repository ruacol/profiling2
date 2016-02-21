Profiling.Views.UsersIndexView = Backbone.View.extend

  templates: 
    table: _.template """
      <table id="users-table" class="table table-bordered table-hover table-condensed">
          <thead>
              <tr>
                  <th>Id</th>
                  <th>User ID</th>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Roles</th>
              </tr>
          </thead>
          <tbody></tbody>
      </table>
    """

  render: ->
    $(@el).html @templates.table()
    _.defer =>
      @initDataTable()
    @

  initDataTable: ->
    @dataTable = new Profiling.DataTable 'users-table',
      bServerSide: false
      sAjaxSource: @options.url
      aaSorting: [[ 1, "asc" ]]
      aoColumnDefs: [{ aTargets: [0], bVisible: false }, { aTargets: [1], sType: 'html' }]
      aoColumns: [ 
        { mDataProp: 'Id' }, 
        { mDataProp: 'UserID' }, 
        { mDataProp: 'UserName' },
        { mDataProp: 'Email' },
        { mDataProp: 'Roles' }
      ]
      fnRowCallback: (nRow, aData, iDisplayIndex) ->
        $("td:eq(0)", nRow).css 'white-space', 'nowrap'
        $("td:eq(0)", nRow).html "<a href='#{Profiling.applicationUrl}System/Users/Details/#{aData['Id']}'>#{aData['UserID']}</a>"
        if aData['Roles'] and _(aData['Roles']).size() > 0
          $("td:eq(3)", nRow).html _(aData['Roles']).rest(1).reduce (x, y) -> 
            x + "<span class='badge'>#{y}</span>"
          , "<span class='badge'>#{_(aData['Roles']).first()}</span>"
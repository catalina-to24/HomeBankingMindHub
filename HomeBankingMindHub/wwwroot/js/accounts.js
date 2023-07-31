var app = new Vue({
    el:"#app",
    data:{
        clientInfo: {},
        loans: [],
        accounts: [],
        error: null
    },
    methods:{
        getData: function () {
            let id = new URLSearchParams(window.location.search).get("id");
            axios.get("/api/clients/"+ id)
            
            .then(function (response) {
                //get client ifo
                console.log(response.data)
                app.accounts = response.data.accounts.$values;
                app.loans = response.data.loans.$values;
                console.log(app.accounts)
                app.clientInfo = response.data;

            })
            .catch(function (error) {
                // handle error
                app.error = error;
            })
        },
        formatDate: function(date){
            return new Date(date).toLocaleDateString('en-gb');
        }
    },
    mounted: function(){
        this.getData();
    }
})
(("undefined"!=typeof self?self:this).webpackJsonp_name_=("undefined"!=typeof self?self:this).webpackJsonp_name_||[]).push([[0],{100:function(e,t,n){},229:function(e,t,n){},230:function(e,t,n){},244:function(e,t,n){"use strict";n.r(t);var o=n(0),a=n.n(o),c=n(7),r=n.n(c),i=(n(100),n(13)),l=n(34),s=n(90);function u(e,t){var n={};return Object.keys(t).map((function(o,a){return n[o]=function(n,a){return function(t,n){return e.dispatch(Object(s.action)(t,n))}(t[o],{path:n,data:a})}})),n}var d=function(e,t,n){var o={method:t,credentials:"same-origin",headers:{"Content-type":"application/json; charset=UTF-8"},body:null};return null!=n&&(o.body=JSON.stringify(n)),fetch(e,o).then((function(e){if(204!=e.status){if(500==e.status)throw e;return e.json()}})).then((function(e){return e})).catch((function(e){return{isError:!0,response:e}}))};function f(e){return d(e,"GET",null)}var p=n(3),m=n.n(p),y=n(33),h=n.n(y),g=n(23),k=n(91),v=n.n(k),E=n(92);function S(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);t&&(o=o.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,o)}return n}function P(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?S(Object(n),!0).forEach((function(t){m()(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):S(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}var O=function(e){return function(t,n){return Object.keys(e).includes(n.type)?function e(t,n,o){if(""==n)return o;var a=n.split("."),c=a.shift();n=a.join(".");var r=t[c],i=e(r||(Array.isArray(t)?[]:{}),n,o);if(Array.isArray(t)){var l=parseInt(c);return[].concat(h()(t.slice(0,l)),[i],h()(t.slice(l+1)))}return P(P({},t),{},m()({},c,i))}(t,n.payload.path,n.payload.data):t}},C=function(e){return Object(E.createLogger)({predicate:function(t,n){return e&&!e.includes(n.type)||!0},level:"info",collapsed:!0})};function A(e,t,n){var o=[v.a,C(n)];return Object(g.createStore)(e,t,g.compose.apply(void 0,[g.applyMiddleware.apply(void 0,o)].concat([])))}function w(e){return e.filter((function(e){return null!=e&&""!=e.trim()})).join(" ")}var N={GET_PLAYLISTS:"GET_PLAYLISTS",GET_PLAYLIST:"GET_PLAYLIST",ADD_TRACK:"ADD_TRACK"},b=A(O(N),{groups:[],playlist:{},currentPlaylist:[]},null),T=u(b,{getPlaylists:N.GET_PLAYLISTS,getPlaylist:N.GET_PLAYLIST,addTrack:N.ADD_TRACK}),L=function(e){return function(){for(var e=arguments.length,t=new Array(e),n=0;n<e;n++)t[n]=arguments[n];return[document.location.origin].concat(t).join("/")}("api/StreamingService",e)},j={getPlaylists:function(){return f(L("playlists")).then((function(e){e&&!e.isError&&T.getPlaylists("groups",e)}))},updatePlaylists:function(e){return f(L("playlists/update?type=".concat(e)))},getPlaylist:function(e,t){return f(L("playlist?type=".concat(e,"&id=").concat(t))).then((function(e){e&&!e.isError&&T.getPlaylist("playlist",e)}))},getSongLink:function(e,t){return L("track?type=".concat(e,"&id=").concat(t))},addToPlaylist:function(e){T.addTrack("currentPlaylist",[e])},addAllToPlaylist:function(e){T.addTrack("currentPlaylist",e)},changeCurrentSong:function(e){var t;t=L("currentSong"),d(t,"POST",{name:e})}},_=n(93),D=n.n(_),H=(n(122),function(e){}),I=function(e){var t=[];e.tracks&&(t=e.tracks.map((function(t){return{name:t.title,singer:t.artist,cover:t.cover,gain:t.gain,musicSrc:function(){return fetch(e.onGetSongLink(t.type,t.id)).then((function(e){return e.text()}))}}})));return a.a.createElement(D.a,{getAudioInstance:function(e){H=function(e){e.crossOrigin="anonymous";var t=new window.AudioContext,n=t.createGain();return n.gain.value=1,t.createMediaElementSource(e).connect(n),n.connect(t.destination),function(e){n.gain.value=1+e/12*.5}}(e)},preload:!1,autoPlay:!1,audioLists:t,mode:"full",onAudioPlay:function(t){var n;n=t.gain,H&&H(n),e.onChangeSong("".concat(t.singer," - ").concat(t.name))},onAudioPause:function(){return e.onChangeSong("Пауза")}})},R=function(e){return a.a.createElement("div",{className:"PlaylistCover"},a.a.createElement("div",{className:"coverContainer"},a.a.createElement("img",{src:"".concat(e.playlist.cover),onClick:e.onClick})),a.a.createElement("div",null,e.playlist.title))},F=function(e){return a.a.createElement("div",{className:"PlaylistRecord"},a.a.createElement("div",{className:"title"},e.track.artist," - ",e.track.title),a.a.createElement("button",{className:"icon-button",onClick:function(){return e.onAdd(e.track)}},a.a.createElement("i",{className:"IconsFont"},"plus")))},U=function(e){return a.a.createElement("div",{className:w(["Form",e.className])},e.children)},G=function(e){return e.playlist?a.a.createElement("div",{className:w(["Playlist",e.visible?"":"hide"])},a.a.createElement("div",{className:"header"},a.a.createElement("div",{className:"info"},a.a.createElement("div",{className:"coverContainer"},a.a.createElement("img",{src:"".concat(e.playlist.cover)})),a.a.createElement("div",{className:"title"},e.playlist.title),a.a.createElement("button",{className:"icon-button",onClick:function(){return e.onAddAll(e.playlist.tracks)}},a.a.createElement("i",{className:"IconsFont"},"plus"))),a.a.createElement("button",{className:"icon-button",onClick:e.onClose},a.a.createElement("i",{className:"IconsFont"},"close"))),a.a.createElement("div",{className:"tracks"},e.playlist.tracks?e.playlist.tracks.map((function(t,n){return a.a.createElement(F,{key:n,track:t,onAdd:e.onAdd})})):null)):null},W=function(e){return a.a.createElement("div",{className:"PlaylistsGroup"},a.a.createElement("div",{className:"header"},function(e){switch(e){case"Local":return"Локальные";case"Yandex":return"Яндекс"}}(e.type),a.a.createElement("button",{className:"icon-button",onClick:function(){return e.onUpdate(e.type)}},a.a.createElement("i",{className:"IconsFont"},"refresh"))),a.a.createElement("div",{className:"content"},e.playlists?e.playlists.map((function(t,n){return a.a.createElement(R,{key:n,playlist:t,onClick:function(){return e.onOpenPlaylist(t.type,t.id)}})})):null))},Y=n(94),K=n.n(Y),x=function(e){var t=Object(o.useState)(!1),n=K()(t,2),c=n[0],r=n[1],i=function(t,n){e.onOpenPlaylist(t,n).then((function(){return r(!c)}))};return a.a.createElement("div",{className:"PlaylistsContainer"},e.groups?e.groups.map((function(t,n){return a.a.createElement(W,{key:n,type:t.group,playlists:t.playlists,onOpenPlaylist:i,onUpdate:e.onUpdatePlaylists})})):null,a.a.createElement(G,{visible:c,playlist:e.playlist,onAdd:e.onAdd,onAddAll:e.onAddAll,onClose:function(){return r(!c)}}))},J=(n(229),function(e){return a.a.createElement(U,null,a.a.createElement(x,{groups:e.groups,playlist:e.playlist,onUpdatePlaylists:function(e){j.updatePlaylists(e).then((function(){return j.getPlaylists()}))},onOpenPlaylist:j.getPlaylist,onAdd:j.addToPlaylist,onAddAll:j.addAllToPlaylist}),a.a.createElement(I,{tracks:e.currentPlaylist,onGetSongLink:j.getSongLink,onChangeSong:j.changeCurrentSong}))}),M=Object(i.connect)((function(e,t){return{groups:e.groups,playlist:e.playlist,currentPlaylist:e.currentPlaylist}}),{})(J),B={UPDATE_FROM_SOCKET:"UPDATE_FROM_SOCKET",CLEAR_SOUND:"CLEAR_SOUND"},$=A(O(B),{currentSong:"Test",sound:""},null),q=function(e){return a.a.createElement("div",{style:{left:"".concat(e.left,"%"),top:"".concat(e.top,"%"),width:"".concat(e.width,"%"),height:"".concat(e.height,"%")},className:w(["Widget",e.className])},e.children)},z=function(e){return a.a.createElement(q,{className:"SongWidget",top:e.top,left:e.left,width:e.width,height:e.height},a.a.createElement("div",{className:"title"},e.song))},Q=function(e){return a.a.createElement(q,{className:"SoundPlayerWidget",top:e.top,left:e.left,width:e.width,height:e.height},a.a.createElement("audio",{autoPlay:!0,src:e.speech,onEnded:function(){e.onPlayEnded()}}))},V=function(e){return a.a.createElement("div",{className:w(["WidgetsContainer",e.className])},e.children)},X=(n(230),u($,{updateFromSocket:B.UPDATE_FROM_SOCKET,clearSound:B.CLEAR_SOUND})),Z=n(21),ee=n.n(Z),te=n(22),ne=n.n(te),oe=new(function(){function e(){ee()(this,e),m()(this,"socketHandlers",void 0),m()(this,"socket",void 0),this.socketHandlers={},this.socket=null}return ne()(e,[{key:"connect",value:function(e){var t=this;null!=this.socket&&this.socket.close(),this.socket=new WebSocket(e),this.socket.onopen=function(){return console.log("Соединение установлено.")},this.socket.onclose=function(e){e.wasClean?console.log("Соединение закрыто чисто"):console.log("Обрыв соединения"),console.log("Код: "+e.code+" причина: "+e.reason)},this.socket.onmessage=function(e){var n=decodeURI(e.data);console.log("Получены данные ".concat(n));var o=JSON.parse(n);t.socketHandlers&&t.socketHandlers[o.event]&&t.socketHandlers[o.event].forEach((function(e){e(o.data)}))},this.socket.onerror=function(e){console.log(e)}}},{key:"disconnect",value:function(){this.socket&&this.socket.readyState!==this.socket.CLOSED&&this.socket.close()}},{key:"send",value:function(e){null!=this.socket&&this.socket.readyState!==this.socket.CLOSED&&this.socket.send(encodeURI(JSON.stringify(e)))}},{key:"on",value:function(e,t){this.socketHandlers[e]||(this.socketHandlers[e]=[]),this.socketHandlers[e].push(t)}},{key:"removeHandler",value:function(e,t){var n=this.socketHandlers[e].indexOf(t);-1!=n&&(this.socketHandlers[e]=this.socketHandlers[e].splice(n,1))}},{key:"onConnect",value:function(e){this.socket&&this.socket.addEventListener("open",e)}},{key:"isConnected",value:function(){return this.socket&&this.socket.readyState===this.socket.OPEN}}]),e}()),ae={isSocketConnected:function(){return oe.isConnected()},socketConnect:function(){var e;oe.connect((e="api/ws","ws://".concat(window.location.host,"/").concat(e)))},socketDisconnect:function(){oe.disconnect()},addSocketHandler:function(e,t){oe.on(e,t)},removeSocketHandler:function(e,t){oe.removeHandler(e,t)},onSocketConnect:function(e){oe.onConnect(e)},socketSend:function(e,t){oe.send({event:e,data:t})},clearSound:function(){X.clearSpeech("sound","")}},ce=function(e){return ae.isSocketConnected()||ae.socketConnect(),ae.addSocketHandler("updateSong",(function(e){X.updateFromSocket("currentSong",e)})),ae.addSocketHandler("speech",(function(e){X.updateFromSocket("sound","api/content/speech?id=".concat(e))})),ae.addSocketHandler("sound",(function(e){X.updateFromSocket("sound","api/content/sound?id=".concat(e))})),ae.onSocketConnect((function(){return ae.socketSend("getCurrentSong",[])})),a.a.createElement(V,null,a.a.createElement(z,{left:0,top:90,width:30,height:10,song:e.currentSong}),a.a.createElement(Q,{left:0,top:90,width:30,height:10,speech:e.speech,onPlayEnded:ae.clearSound}))},re=Object(i.connect)((function(e,t){return{currentSong:e.currentSong,speech:e.sound}}),{})(ce),ie=[{path:"/",component:function(){return j.getPlaylists(),a.a.createElement(i.Provider,{store:b},a.a.createElement(M,null))}},{path:"/stream",component:function(){return a.a.createElement(i.Provider,{store:$},a.a.createElement(re,null))}}],le=function(){return a.a.createElement(l.BrowserRouter,null,a.a.createElement(l.Switch,null,ie.map((function(e,t){return a.a.createElement(l.Route,{exact:!0,path:e.path,component:e.component})}))))},se=n(95),ue=document.getElementById("root");r.a.render(a.a.createElement(le,null),ue),se.a()},95:function(e,t,n){"use strict";(function(e){n.d(t,"a",(function(){return o}));Boolean("localhost"===window.location.hostname||"[::1]"===window.location.hostname||window.location.hostname.match(/^127(?:\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$/));function o(){"serviceWorker"in navigator&&navigator.serviceWorker.ready.then((function(e){e.unregister()})).catch((function(e){console.error(e.message)}))}}).call(this,n(243))}}]);
(("undefined"!=typeof self?self:this).webpackJsonp_name_=("undefined"!=typeof self?self:this).webpackJsonp_name_||[]).push([[0],{100:function(t,e,n){},229:function(t,e,n){},230:function(t,e,n){},244:function(t,e,n){"use strict";n.r(e);var a=n(0),o=n.n(a),r=n(7),c=n.n(r),l=(n(100),n(13)),i=n(34),s=n(90);function u(t,e){var n={};return Object.keys(e).map((function(a,o){return n[a]=function(n,o){return function(e,n){return t.dispatch(Object(s.action)(e,n))}(e[a],{path:n,data:o})}})),n}var d=function(t,e,n){var a={method:e,credentials:"same-origin",headers:{"Content-type":"application/json; charset=UTF-8"},body:null};return null!=n&&(a.body=JSON.stringify(n)),fetch(t,a).then((function(t){if(204!=t.status){if(500==t.status)throw t;return t.json()}})).then((function(t){return t})).catch((function(t){return{isError:!0,response:t}}))};function f(t){return d(t,"GET",null)}var p=n(3),m=n.n(p),y=n(33),h=n.n(y),g=n(23),k=n(91),v=n.n(k),E=n(92);function S(t,e){var n=Object.keys(t);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(t);e&&(a=a.filter((function(e){return Object.getOwnPropertyDescriptor(t,e).enumerable}))),n.push.apply(n,a)}return n}function P(t){for(var e=1;e<arguments.length;e++){var n=null!=arguments[e]?arguments[e]:{};e%2?S(Object(n),!0).forEach((function(e){m()(t,e,n[e])})):Object.getOwnPropertyDescriptors?Object.defineProperties(t,Object.getOwnPropertyDescriptors(n)):S(Object(n)).forEach((function(e){Object.defineProperty(t,e,Object.getOwnPropertyDescriptor(n,e))}))}return t}var O=function(t){return function(e,n){return Object.keys(t).includes(n.type)?function t(e,n,a){if(""==n)return a;var o=n.split("."),r=o.shift();n=o.join(".");var c=e[r],l=t(c||(Array.isArray(e)?[]:{}),n,a);if(Array.isArray(e)){var i=parseInt(r);return[].concat(h()(e.slice(0,i)),[l],h()(e.slice(i+1)))}return P(P({},e),{},m()({},r,l))}(e,n.payload.path,n.payload.data):e}},A=function(t){return Object(E.createLogger)({predicate:function(e,n){return t&&!t.includes(n.type)||!0},level:"info",collapsed:!0})};function b(t,e,n){var a=[v.a,A(n)];return Object(g.createStore)(t,e,g.compose.apply(void 0,[g.applyMiddleware.apply(void 0,a)].concat([])))}function T(t){return t.filter((function(t){return null!=t&&""!=t.trim()})).join(" ")}var w={GET_PLAYLISTS:"GET_PLAYLISTS",GET_PLAYLIST:"GET_PLAYLIST",ADD_TRACK:"ADD_TRACK"},C=b(O(w),{playlists:[],playlist:{},currentPlaylist:[]},null),N=u(C,{getPlaylists:w.GET_PLAYLISTS,getPlaylist:w.GET_PLAYLIST,addTrack:w.ADD_TRACK}),j=function(t){return function(){for(var t=arguments.length,e=new Array(t),n=0;n<t;n++)e[n]=arguments[n];return[document.location.origin].concat(e).join("/")}("api/StreamingService",t)},L={getPlaylists:function(){return f(j("playlists")).then((function(t){t&&!t.isError&&N.getPlaylists("playlists",t)}))},getPlaylist:function(t,e){return f(j("playlist?type=".concat(t,"&id=").concat(e))).then((function(t){t&&!t.isError&&N.getPlaylist("playlist",t)}))},getSongLink:function(t,e){return j("track?type=".concat(t,"&id=").concat(e))},addToPlaylist:function(t){N.addTrack("currentPlaylist",[t])},addAllToPlaylist:function(t){N.addTrack("currentPlaylist",t)},changeCurrentSong:function(t){var e;e=j("currentSong"),d(e,"POST",{name:t})}},_=n(93),D=n.n(_),I=(n(122),function(t){var e=[];return t.tracks&&(e=t.tracks.map((function(e){return{name:e.title,singer:e.artist,cover:e.cover,musicSrc:function(){return fetch(t.onGetSongLink(e.type,e.id)).then((function(t){return t.text()})).then((function(t){return t}))}}}))),o.a.createElement(D.a,{preload:!1,autoPlay:!1,audioLists:e,mode:"full",onAudioPlay:function(e){return t.onChangeSong("".concat(e.singer," - ").concat(e.name))},onAudioPause:function(){return t.onChangeSong("Пауза")}})}),H=function(t){return o.a.createElement("div",{className:"PlaylistCover"},o.a.createElement("img",{src:"".concat(t.playlist.cover),onClick:t.onClick}),o.a.createElement("div",null,t.playlist.title))},R=function(t){return o.a.createElement("div",{className:"PlaylistRecord"},o.a.createElement("div",{className:"title"},t.track.artist," - ",t.track.title),o.a.createElement("button",{className:"icon-button",onClick:function(){return t.onAdd(t.track)}},o.a.createElement("i",{className:"IconsFont"},"plus")))},F=function(t){return o.a.createElement("div",{className:T(["Form",t.className])},t.children)},G=function(t){return t.playlist?o.a.createElement("div",{className:T(["Playlist",t.visible?"":"hide"])},o.a.createElement("div",{className:"header"},o.a.createElement("div",{className:"info"},o.a.createElement("img",{src:"".concat(t.playlist.cover)}),o.a.createElement("div",{className:"title"},t.playlist.title),o.a.createElement("button",{className:"icon-button",onClick:function(){return t.onAddAll(t.playlist.tracks)}},o.a.createElement("i",{className:"IconsFont"},"plus"))),o.a.createElement("button",{className:"icon-button",onClick:t.onClose},o.a.createElement("i",{className:"IconsFont"},"close"))),o.a.createElement("div",{className:"tracks"},t.playlist.tracks?t.playlist.tracks.map((function(e,n){return o.a.createElement(R,{key:n,track:e,onAdd:t.onAdd})})):null)):null},K=n(94),U=n.n(K),W=function(t){var e=Object(a.useState)(!1),n=U()(e,2),r=n[0],c=n[1];return o.a.createElement("div",{className:"PlaylistsContainer"},t.playlists?t.playlists.map((function(e,n){return o.a.createElement(H,{key:n,playlist:e,onClick:function(){return n=e.type,a=e.id,void t.onOpenPlaylist(n,a).then((function(){return c(!r)}));var n,a}})})):null,o.a.createElement(G,{visible:r,playlist:t.playlist,onAdd:t.onAdd,onAddAll:t.onAddAll,onClose:function(){return c(!r)}}))},Y=(n(229),function(t){return o.a.createElement(F,null,o.a.createElement(W,{playlists:t.playlists,playlist:t.playlist,onOpenPlaylist:L.getPlaylist,onAdd:L.addToPlaylist,onAddAll:L.addAllToPlaylist}),o.a.createElement(I,{tracks:t.currentPlaylist,onGetSongLink:L.getSongLink,onChangeSong:L.changeCurrentSong}))}),J=Object(l.connect)((function(t,e){return{playlists:t.playlists,playlist:t.playlist,currentPlaylist:t.currentPlaylist}}),{})(Y),M={UPDATE_FROM_SOCKET:"UPDATE_FROM_SOCKET"},x=b(O(M),{currentSong:"Test"},null),B=function(t){return o.a.createElement("div",{style:{left:"".concat(t.left,"%"),top:"".concat(t.top,"%"),width:"".concat(t.width,"%"),height:"".concat(t.height,"%")},className:T(["Widget",t.className])},t.children)},$=function(t){return o.a.createElement(B,{className:"SongWidget",top:t.top,left:t.left,width:t.width,height:t.height},o.a.createElement("div",{className:"title"},t.song))},q=function(t){return o.a.createElement("div",{className:T(["WidgetsContainer",t.className])},t.children)},z=(n(230),u(x,{updateFromSocket:M.UPDATE_FROM_SOCKET})),Q=n(21),V=n.n(Q),X=n(22),Z=n.n(X),tt=new(function(){function t(){V()(this,t),m()(this,"socketHandlers",void 0),m()(this,"socket",void 0),this.socketHandlers={},this.socket=null}return Z()(t,[{key:"connect",value:function(t){var e=this;null!=this.socket&&this.socket.close(),this.socket=new WebSocket(t),this.socket.onopen=function(){return console.log("Соединение установлено.")},this.socket.onclose=function(t){t.wasClean?console.log("Соединение закрыто чисто"):console.log("Обрыв соединения"),console.log("Код: "+t.code+" причина: "+t.reason)},this.socket.onmessage=function(t){var n=decodeURI(t.data);console.log("Получены данные ".concat(n));var a=JSON.parse(n);e.socketHandlers&&e.socketHandlers[a.event]&&e.socketHandlers[a.event].forEach((function(t){t(a.data)}))},this.socket.onerror=function(t){console.log(t)}}},{key:"disconnect",value:function(){this.socket&&this.socket.readyState!==this.socket.CLOSED&&this.socket.close()}},{key:"send",value:function(t){null!=this.socket&&this.socket.readyState!==this.socket.CLOSED&&this.socket.send(encodeURI(JSON.stringify(t)))}},{key:"on",value:function(t,e){this.socketHandlers[t]||(this.socketHandlers[t]=[]),this.socketHandlers[t].push(e)}},{key:"removeHandler",value:function(t,e){var n=this.socketHandlers[t].indexOf(e);-1!=n&&(this.socketHandlers[t]=this.socketHandlers[t].splice(n,1))}},{key:"isConnected",value:function(){return this.socket&&this.socket.readyState===this.socket.OPEN}}]),t}()),et=function(){return tt.isConnected()},nt=function(){var t;tt.connect((t="api/ws","ws://".concat(window.location.host,"/").concat(t)))},at=function(t,e){tt.on(t,e)},ot=function(t){return et()||nt(),at("updateSong",(function(t){z.updateFromSocket("currentSong",t),console.log("Новая песня: ".concat(t))})),o.a.createElement(q,null,o.a.createElement($,{left:0,top:90,width:30,height:10,song:t.currentSong}))},rt=Object(l.connect)((function(t,e){return{currentSong:t.currentSong}}),{})(ot),ct=[{path:"/",component:function(){return L.getPlaylists(),o.a.createElement(l.Provider,{store:C},o.a.createElement(J,null))}},{path:"/stream",component:function(){return o.a.createElement(l.Provider,{store:x},o.a.createElement(rt,null))}}],lt=function(){return o.a.createElement(i.BrowserRouter,null,o.a.createElement(i.Switch,null,ct.map((function(t,e){return o.a.createElement(i.Route,{exact:!0,path:t.path,component:t.component})}))))},it=n(95),st=document.getElementById("root");c.a.render(o.a.createElement(lt,null),st),it.a()},95:function(t,e,n){"use strict";(function(t){n.d(e,"a",(function(){return a}));Boolean("localhost"===window.location.hostname||"[::1]"===window.location.hostname||window.location.hostname.match(/^127(?:\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$/));function a(){"serviceWorker"in navigator&&navigator.serviceWorker.ready.then((function(t){t.unregister()})).catch((function(t){console.error(t.message)}))}}).call(this,n(243))}}]);
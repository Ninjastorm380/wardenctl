pkgname=wardenctl
pkgver=0.0.1
pkgrel=1
epoch=1
pkgdesc=""
arch=(x86_64)
url=""
license=('unknown')
groups=()
depends=(rampartfs)
makedepends=(dotnet-host dotnet-runtime dotnet-targeting-pack netstandard-targeting-pack dotnet-sdk git)
checkdepends=()
optdepends=()
provides=()
conflicts=()
replaces=()
backup=()
options=(!strip)
install=
changelog=
source=()
noextract=()
sha256sums=()
validpgpkeys=()

prepare() {
	git clone "https://github.com/Ninjastorm380/wardenctl.git"
}

build() {
    cd $srcdir/wardenctl
	dotnet publish $srcdir/wardenctl -c release
}

check() {
	return 0
}

package() {
	mkdir $pkgdir/usr
	mkdir $pkgdir/usr/bin

	cp $srcdir/wardenctl/wardenctl/bin/Release/net9.0/linux-x64/publish/wardenctl $pkgdir/usr/bin/wardenctl
}
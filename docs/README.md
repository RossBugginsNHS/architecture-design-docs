[See docs](/docs/index.md)

Jekyll Ruby pre reqs

```
sudo apt-get install ruby-full build-essential zlib1g-dev
sudo gem install jekyll bundler
```

Install node and npm

Install npm reqs

```
npm install --save-dev webpack

bundle install
```

> Whether to use --save-dev or not depends on your use cases. 
> Say you're using webpack only for bundling, then it's suggested that you install it with --save-dev option since you're not going to include webpack in your production build. 
> Otherwise you can ignore --save-dev.

To Build

```
npm run build-development
```

This will build the node modules cache and run wepack and builds jekyll

To then run locally

```
npm run debug
```

To run without the npm config, the below can be used, but all config won't be loaded
```
bundle exec jekyll serve
```
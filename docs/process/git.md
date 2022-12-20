---
title: Git
layout: home
nav_order: 14
parent: Contributing
---



# Git

## Commits

### Commit pgp signing

You can sign commits and tags locally, to give other people confidence about the origin of a change you have made. If a commit or tag has a GPG or S/MIME signature that is cryptographically verifiable, GitHub marks the commit or tag "Verified" or "Partially verified."

**For this repo, please ensure all commits are PGP signed.**
[Github PGP signing guidance](https://docs.github.com/en/authentication/managing-commit-signature-verification/signing-commits)

### More info

To configure your Git client to sign commits by default for a local repository, in Git versions 2.0.0 and above, run `git config commit.gpgsign true`. To sign all commits by default in any local repository on your computer, run `git config --global commit.gpgsign true`.

To store your GPG key passphrase so you don't have to enter it every time you sign a commit, we recommend using the following tools:
  - For Mac users, the [GPG Suite](https://gpgtools.org/) allows you to store your GPG key passphrase in the Mac OS Keychain.
  - For Windows users, the [Gpg4win](https://www.gpg4win.org/) integrates with other Windows tools.

You can also manually configure [gpg-agent](http://linux.die.net/man/1/gpg-agent) to save your GPG key passphrase, but this doesn't integrate with Mac OS Keychain like ssh-agent and requires more setup.


If you have multiple keys or are attempting to sign commits or tags with a key that doesn't match your committer identity, you should [tell Git about your signing key](https://docs.github.com/en/articles/telling-git-about-your-signing-key).

1. When committing changes in your local branch, add the -S flag to the git commit command:
  ```shell
  $ git commit -S -m <em>"your commit message"</em>
  # Creates a signed commit
  ```
2. If you're using GPG, after you create your commit, provide the passphrase you set up when you [generated your GPG key](https://docs.github.com/en/articles/generating-a-new-gpg-key).
3. When you've finished creating commits locally, push them to your remote repository on github:
  ```shell
  $ git push
  # Pushes your local commits to the remote repository
  ```
4. On github, navigate to your pull request.
5. To view more detailed information about the verified signature, click Verified.
![Signed commit](https://docs.github.com/en/assets/images/help/commits/gpg-signed-commit-verified-without-details.png)

## Further reading

* "[Checking for existing GPG keys](https://docs.github.com/en/articles/checking-for-existing-gpg-keys)"
* "[Generating a new GPG key](https://docs.github.com/en/articles/generating-a-new-gpg-key)"
* "[Adding a new GPG key to your GitHub account](https://docs.github.com/en/articles/adding-a-new-gpg-key-to-your-github-account)"
* "[Telling Git about your signing key](https://docs.github.com/en/articles/telling-git-about-your-signing-key)"
* "[Associating an email with your GPG key](https://docs.github.com/en/articles/associating-an-email-with-your-gpg-key)"
* "[Signing tags](https://docs.github.com/en/articles/signing-tags)"